USE [SRTSWeb]
GO
/****** Object:  UserDefinedFunction [dbo].[It_Depends]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[It_Depends] (@ObjectName Varchar(200), @ObjectsOnWhichItDepends bit)

RETURNS @References TABLE (

       ThePath VARCHAR(MAX), --the ancestor objects delimited by a '/'

       TheFullEntityName VARCHAR(200),

       TheType VARCHAR(20),

       iteration INT )

 

/**

summary:   >

 This Table function returns a a table giving the dependencies of the object whose name

 is supplied as a parameter.

 At the moment, only objects are allowed as a parameter, You can specify whether you 

 want those objects that rely on the object, or those on whom the object relies.

compatibility: SQL Server 2005 - SQL Server 2012

 Revisions:

 - Author: Phil Factor

   Version: 1.1

   Modification: Allowed both types of dependencies, returned full detail table

   date: 20 Sep 2015

ToDo: Must add assemblies, must allow entities such as types to be specified

example:

     - code: |

Use AdventureWorks

SELECT  space(iteration * 4) + TheFullEntityName + ' (' + rtrim(TheType) + ')'

FROM    dbo.It_Depends('Employee',0)

ORDER BY ThePath

     - code: |

Select * from dbo.It_Depends('Employee',1)

Select * from dbo.It_Depends('Employee',0)

returns:   >

@references table, which has the name, the type, the display order and th

e 'path' of each dependent object

 

**/

AS

BEGIN

DECLARE    @DatabaseDependencies  TABLE (

       EntityName  VARCHAR(200),

       EntityType CHAR(5),

       DependencyType  CHAR(4),

       TheReferredEntity VARCHAR(200),

       TheReferredType  CHAR(5) ) 

 

INSERT   INTO  @DatabaseDependencies  ( EntityName, EntityType, DependencyType, TheReferredEntity, TheReferredType )

              -- tables that reference udts

        SELECT  object_schema_name(o.object_id) + '.' + o.name, o.type, 'hard', ty.name, 'UDT'

        FROM    sys.objects o

                INNER JOIN sys.columns AS c ON c.object_ID = o.object_id

                INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id

        WHERE   is_user_defined  = 1

        UNION ALL

              -- udtts that reference udts

        SELECT  object_schema_name(tt.type_table_object_id) + '.' + tt.name, 'UDTT', 'hard', ty.name, 'UDT'

        FROM    sys.table_types tt

                INNER JOIN sys.columns AS c ON c.object_id = tt.type_table_object_id

                INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id

        WHERE   ty.is_user_defined = 1

         UNION ALL

              --tables/views that reference triggers          

        SELECT  object_schema_name(o.object_id) + '.' + o.name, o.type, 'hard', object_schema_name(t.object_id) + '.' + t.name, t.type

        FROM    sys.objects t

                INNER JOIN sys.objects AS o ON o.parent_object_id = t.object_id

        WHERE   o.type = 'TR'

        UNION ALL

              -- tables that reference defaults via columns (only default objects)

        SELECT  object_schema_name(clmns.object_id) + '.' + object_name(clmns.object_id), 'U', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.columns AS clmns ON clmns.default_object_id = o.object_id

        WHERE   o.parent_object_id = 0

        UNION ALL

              -- types that reference defaults (only default objects)

        SELECT  types.name, 'UDT', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.types AS types ON types.default_object_id = o.object_id

        WHERE   o.parent_object_id = 0

        UNION ALL

              -- tables that reference rules via columns 

        SELECT  object_schema_name(clmns.object_id) + '.' + object_name(clmns.object_id), 'U', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.columns AS clmns ON clmns.rule_object_id = o.object_id

        UNION ALL           

              -- types that reference rules  

        SELECT  types.name, 'UDT', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.types AS types ON types.rule_object_id = o.object_id

        UNION ALL

              -- tables that reference XmlSchemaCollections

        SELECT  object_schema_name(clmns.object_id) + '.' + object_name(clmns.object_id), 'U', 'hard', xml_schema_collections.name, 'XMLC'

        FROM    sys.columns clmns --should we eliminate views?

                INNER JOIN sys.xml_schema_collections ON  xml_schema_collections.xml_collection_id = clmns.xml_collection_id

        UNION ALL

              -- table types that reference XmlSchemaCollections

        SELECT  object_schema_name(clmns.object_id) + '.' + object_name(clmns.object_id), 'UDTT', 'hard', xml_schema_collections.name, 'XMLC'

        FROM    sys.columns AS clmns

                INNER JOIN sys.table_types AS tt ON tt.type_table_object_id = clmns.object_id

                INNER JOIN sys.xml_schema_collections ON  xml_schema_collections.xml_collection_id = clmns.xml_collection_id

        UNION ALL

              -- procedures that reference XmlSchemaCollections

        SELECT  object_schema_name(params.object_id) + '.' + o.name, o.type, 'hard', xml_schema_collections.name, 'XMLC'

        FROM    sys.parameters AS params

                INNER JOIN sys.xml_schema_collections ON  xml_schema_collections.xml_collection_id = params.xml_collection_id

                INNER JOIN sys.objects o ON o.object_id = params.object_id

        UNION ALL

              -- table references table

        SELECT  object_schema_name(tbl.object_id) + '.' + tbl.name, tbl.type, 'hard', object_schema_name(referenced_object_id) + '.' + object_name(referenced_object_id), 'U'

        FROM    sys.foreign_keys AS fk

                INNER JOIN sys.tables AS tbl ON tbl.object_id = fk.parent_object_id

        UNION ALL                 

 

              -- uda references types

        SELECT  object_schema_name(params.object_id) + '.' + o.name, o.type, 'hard', types.name, 'UDT'

        FROM    sys.parameters AS params

                INNER JOIN sys.types ON types.user_type_id = params.user_type_id

                INNER JOIN sys.objects o ON o.object_id = params.object_id

        WHERE   is_user_defined  <> 0

        UNION ALL

 

              -- table,view references partition scheme

        SELECT  object_schema_name(o.object_id) + '.' + o.name, o.type, 'hard', ps.name, 'PS'

        FROM    sys.indexes AS idx

                INNER JOIN sys.partitions p ON idx.object_id = p.object_id AND idx.index_id = p.index_id

                INNER JOIN sys.partition_schemes ps ON idx.data_space_id = ps.data_space_id

                INNER JOIN sys.objects AS o ON o.object_id = idx.object_id

        UNION ALL

 

              -- partition scheme references partition function

        SELECT  ps.name, 'PS', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.partition_schemes ps

                INNER JOIN sys.objects AS o ON ps.function_id = o.object_id

        UNION ALL          

 

              -- plan guide references sp, udf, triggers

        SELECT  pg.name, 'PG', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.plan_guides AS pg ON pg.scope_object_id = o.object_id

        UNION ALL

 

              -- synonym refrences object

        SELECT  s.name, 'SYN', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.synonyms AS s ON object_id(s.base_object_name) = o.object_id

        UNION ALL                        

              

              --   sequences that reference uddts 

        SELECT  s.name, 'SYN', 'hard', object_schema_name(o.object_id) + '.' + o.name, o.type

        FROM    sys.objects o

                INNER JOIN sys.sequences AS s ON s.user_type_id = o.object_id

        UNION ALL

        SELECT DISTINCT

                coalesce(object_schema_name(Referencing_ID) + '.', '') + object_name(Referencing_ID), referencer.type, 'soft', coalesce(referenced_schema_name + '.', '') + --likely schema name

         coalesce(referenced_entity_name, ''), --very likely entity name

                referenced.type

        FROM    sys.sql_expression_dependencies

                INNER JOIN sys.objects referencer ON referencing_id = referencer.object_ID

                INNER JOIN sys.objects referenced ON referenced_id = referenced.object_ID

        WHERE   referencing_Class = 1 AND referenced_class = 1 AND referencer.type IN  ( 'v', 'tf', 'fn', 'p', 'tr', 'u' )

 

DECLARE @RowCount INT

DECLARE @ii INT

-- firstly we put in the object as a seed.

INSERT   INTO @References ( ThePath, TheFullEntityName, theType, iteration )

        SELECT  coalesce(object_schema_name(object_ID) + '.', '') + name, coalesce(object_schema_name(object_ID) + '.', '') + name, type, 1

        FROM    sys.objects WHERE name LIKE @ObjectName

-- then we just pull out the dependencies at each level. watching out for

-- self-references and circular references

SELECT   @rowcount = @@ROWCOUNT, @ii = 2

IF @ObjectsOnWhichItDepends<>0 --if we are looking for objects on which it depends

WHILE @ii < 20 AND @rowcount > 0

   BEGIN

    INSERT  INTO @References ( ThePath, TheFullEntityName, theType, iteration )

            SELECT DISTINCT

                    ThePath + '/' + TheReferredEntity, TheReferredEntity, TheReferredType, @ii

            FROM    @DatabaseDependencies DatabaseDependencies

                    INNER JOIN @References  previousReferences 

                                   ON previousReferences.TheFullEntityName = EntityName  

                                    AND previousReferences.iteration = @ii - 1

                     WHERE  TheReferredEntity<>EntityName

                     AND  TheReferredEntity NOT IN  (SELECT TheFullEntityName FROM @References)

    SELECT  @rowcount = @@rowcount

    SELECT  @ii = @ii + 1

   END

ELSE --we are looking for objects that depend on it.

WHILE @ii < 20 AND @rowcount > 0

   BEGIN

    INSERT  INTO @References ( ThePath, TheFullEntityName, theType, iteration )

            SELECT DISTINCT

                    ThePath + '/' + EntityName, EntityName, TheType, @ii

            FROM    @DatabaseDependencies DatabaseDependencies

                    INNER JOIN @References  previousReferences 

                                   ON previousReferences.TheFullEntityName = TheReferredEntity  

                                   AND previousReferences.iteration = @ii - 1

                     WHERE  TheReferredEntity<>EntityName

                     AND EntityName NOT IN (SELECT TheFullEntityName FROM @References)

    SELECT  @rowcount = @@rowcount

    SELECT  @ii = @ii + 1

   END

RETURN

 END
GO
/****** Object:  UserDefinedFunction [dbo].[SplitStrings]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitStrings]
(
    @List       VARCHAR(MAX),
    @Delimiter  VARCHAR(255)
)
RETURNS TABLE
AS
    RETURN (SELECT Number = ROW_NUMBER() OVER (ORDER BY Number),
        Item FROM (SELECT Number, Item = LTRIM(RTRIM(SUBSTRING(@List, Number, 
        CHARINDEX(@Delimiter, @List + @Delimiter, Number) - Number)))
    FROM (SELECT ROW_NUMBER() OVER (ORDER BY s1.[object_id])
        FROM sys.all_objects AS s1 CROSS APPLY sys.all_objects) AS n(Number)
    WHERE Number <= CONVERT(INT, LEN(@List))
        AND SUBSTRING(@Delimiter + @List, Number, 1) = @Delimiter
    ) AS y);
GO
/****** Object:  UserDefinedFunction [dbo].[PROPERCASE]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[PROPERCASE]
(
--The string to be converted to proper case
@input varchar(8000)
)
--This function returns the proper case string of varchar type
RETURNS varchar(8000)
AS
BEGIN
	IF @input IS NULL 
	BEGIN
		--Just return NULL if input string is NULL
		RETURN NULL
	END
	
	--Character variable declarations
	DECLARE @output varchar(8000)
	--Integer variable declarations
	DECLARE @ctr int, @len int, @found_at int
	--Constant declarations
	DECLARE @LOWER_CASE_a int, @LOWER_CASE_z int, @Delimiter char(3), @UPPER_CASE_A int, @UPPER_CASE_Z int
	
	--Variable/Constant initializations
	SET @ctr = 1
	SET @len = LEN(@input)
	SET @output = ''
	SET @LOWER_CASE_a = 97
	SET @LOWER_CASE_z = 122
	SET @Delimiter = ' ,-'
	SET @UPPER_CASE_A = 65
	SET @UPPER_CASE_Z = 90
	
	WHILE @ctr <= @len
	BEGIN
		--This loop will take care of reccuring white spaces
		WHILE CHARINDEX(SUBSTRING(@input,@ctr,1), @Delimiter) > 0
		BEGIN
			SET @output = @output + SUBSTRING(@input,@ctr,1)
			SET @ctr = @ctr + 1
		END

		IF ASCII(SUBSTRING(@input,@ctr,1)) BETWEEN @LOWER_CASE_a AND @LOWER_CASE_z
		BEGIN
			--Converting the first character to upper case
			SET @output = @output + UPPER(SUBSTRING(@input,@ctr,1))
		END
		ELSE
		BEGIN
			SET @output = @output + SUBSTRING(@input,@ctr,1)
		END
		
		SET @ctr = @ctr + 1

		WHILE CHARINDEX(SUBSTRING(@input,@ctr,1), @Delimiter) = 0 AND (@ctr <= @len)
		BEGIN
			IF ASCII(SUBSTRING(@input,@ctr,1)) BETWEEN @UPPER_CASE_A AND @UPPER_CASE_Z
			BEGIN
				SET @output = @output + LOWER(SUBSTRING(@input,@ctr,1))
			END
			ELSE
			BEGIN
				SET @output = @output + SUBSTRING(@input,@ctr,1)
			END
			SET @ctr = @ctr + 1
		END
		
	END
RETURN @output
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Setup_RestorePermissions]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Setup_RestorePermissions]
    @name   sysname
AS
BEGIN
    DECLARE @object sysname
    DECLARE @protectType char(10)
    DECLARE @action varchar(60)
    DECLARE @grantee sysname
    DECLARE @cmd nvarchar(500)
    DECLARE c1 cursor FORWARD_ONLY FOR
        SELECT Object, ProtectType, [Action], Grantee FROM #aspnet_Permissions where Object = @name

    OPEN c1

    FETCH c1 INTO @object, @protectType, @action, @grantee
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = @protectType + ' ' + @action + ' on ' + @object + ' TO [' + @grantee + ']'
        EXEC (@cmd)
        FETCH c1 INTO @object, @protectType, @action, @grantee
    END

    CLOSE c1
    DEALLOCATE c1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Setup_RemoveAllRoleMembers]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Setup_RemoveAllRoleMembers]
    @name   sysname
AS
BEGIN
    CREATE TABLE #aspnet_RoleMembers
    (
        Group_name      sysname,
        Group_id        smallint,
        Users_in_group  sysname,
        User_id         smallint
    )

    INSERT INTO #aspnet_RoleMembers
    EXEC sp_helpuser @name

    DECLARE @user_id smallint
    DECLARE @cmd nvarchar(500)
    DECLARE c1 cursor FORWARD_ONLY FOR
        SELECT User_id FROM #aspnet_RoleMembers

    OPEN c1

    FETCH c1 INTO @user_id
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = 'EXEC sp_droprolemember ' + '''' + @name + ''', ''' + USER_NAME(@user_id) + ''''
        EXEC (@cmd)
        FETCH c1 INTO @user_id
    END

    CLOSE c1
    DEALLOCATE c1
END
GO
/****** Object:  UserDefinedFunction [dbo].[Base64Decode]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[Base64Decode] ( @Input NVARCHAR(MAX) )
 
RETURNS VARCHAR(MAX)
BEGIN
 
DECLARE @DecodedOutput VARCHAR(MAX)
 
set @DecodedOutput = CAST(CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@Input"))', 'VARBINARY(MAX)') AS NVARCHAR(MAX))
 
RETURN @DecodedOutput
 
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UnRegisterSchemaVersion]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UnRegisterSchemaVersion]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128)
AS
BEGIN
    DELETE FROM dbo.aspnet_SchemaVersions
        WHERE   Feature = LOWER(@Feature) AND @CompatibleSchemaVersion = CompatibleSchemaVersion
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_WebEvent_LogEvent]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_WebEvent_LogEvent]
        @EventId         char(32),
        @EventTimeUtc    datetime,
        @EventTime       datetime,
        @EventType       nvarchar(256),
        @EventSequence   decimal(19,0),
        @EventOccurrence decimal(19,0),
        @EventCode       int,
        @EventDetailCode int,
        @Message         nvarchar(1024),
        @ApplicationPath nvarchar(256),
        @ApplicationVirtualPath nvarchar(256),
        @MachineName    nvarchar(256),
        @RequestUrl      nvarchar(1024),
        @ExceptionType   nvarchar(256),
        @Details         ntext
AS
BEGIN
    INSERT
        dbo.aspnet_WebEvent_Events
        (
            EventId,
            EventTimeUtc,
            EventTime,
            EventType,
            EventSequence,
            EventOccurrence,
            EventCode,
            EventDetailCode,
            Message,
            ApplicationPath,
            ApplicationVirtualPath,
            MachineName,
            RequestUrl,
            ExceptionType,
            Details
        )
    VALUES
    (
        @EventId,
        @EventTimeUtc,
        @EventTime,
        @EventType,
        @EventSequence,
        @EventOccurrence,
        @EventCode,
        @EventDetailCode,
        @Message,
        @ApplicationPath,
        @ApplicationVirtualPath,
        @MachineName,
        @RequestUrl,
        @ExceptionType,
        @Details
    )
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteFrameDefaults]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 Dec 2016
-- Description:	Use to delete Frame Defaults based upon a
--			specific frame.
-- =============================================
CREATE PROCEDURE [dbo].[DeleteFrameDefaults]
	@FrameCode VarChar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from dbo.FrameDefaults
	where FrameCode = @FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteAuthorizationByUserName]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteAuthorizationByUserName]
	-- Add the parameters for the stored procedure here
@UserName varchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
						
			DELETE FROM [Authorization]
			WHERE UserName = @UserName;
			
			IF Exists
			( 
				SELECT TOP 1 UserName FROM [Authorization] WHERE SSO_UserName = @UserName
			)
			BEGIN
				Update [Authorization] SET SSO_UserName = (SELECT TOP 1 UserName FROM [Authorization] WHERE SSO_UserName = @UserName)
				WHERE SSO_UserName = @UserName
			END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_CheckSchemaVersion]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_CheckSchemaVersion]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128)
AS
BEGIN
    IF (EXISTS( SELECT  *
                FROM    dbo.aspnet_SchemaVersions
                WHERE   Feature = LOWER( @Feature ) AND
                        CompatibleSchemaVersion = @CompatibleSchemaVersion ))
        RETURN 0

    RETURN 1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Applications_CreateApplication]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Applications_CreateApplication]
    @ApplicationName      nvarchar(256),
    @ApplicationId        uniqueidentifier OUTPUT
AS
BEGIN
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName

    IF(@ApplicationId IS NULL)
    BEGIN
        DECLARE @TranStarted   bit
        SET @TranStarted = 0

        IF( @@TRANCOUNT = 0 )
        BEGIN
	        BEGIN TRANSACTION
	        SET @TranStarted = 1
        END
        ELSE
    	    SET @TranStarted = 0

        SELECT  @ApplicationId = ApplicationId
        FROM dbo.aspnet_Applications WITH (UPDLOCK, HOLDLOCK)
        WHERE LOWER(@ApplicationName) = LoweredApplicationName

        IF(@ApplicationId IS NULL)
        BEGIN
            SELECT  @ApplicationId = NEWID()
            INSERT  dbo.aspnet_Applications (ApplicationId, ApplicationName, LoweredApplicationName)
            VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName))
        END


        IF( @TranStarted = 1 )
        BEGIN
            IF(@@ERROR = 0)
            BEGIN
	        SET @TranStarted = 0
	        COMMIT TRANSACTION
            END
            ELSE
            BEGIN
                SET @TranStarted = 0
                ROLLBACK TRANSACTION
            END
        END
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Applications_ALTERApplication]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Applications_ALTERApplication]
    @ApplicationName      nvarchar(256),
    @ApplicationId        uniqueidentifier OUTPUT
AS
BEGIN
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName

    IF(@ApplicationId IS NULL)
    BEGIN
        DECLARE @TranStarted   bit
        SET @TranStarted = 0

        IF( @@TRANCOUNT = 0 )
        BEGIN
	        BEGIN TRANSACTION
	        SET @TranStarted = 1
        END
        ELSE
    	    SET @TranStarted = 0

        SELECT  @ApplicationId = ApplicationId
        FROM dbo.aspnet_Applications WITH (UPDLOCK, HOLDLOCK)
        WHERE LOWER(@ApplicationName) = LoweredApplicationName

        IF(@ApplicationId IS NULL)
        BEGIN
            SELECT  @ApplicationId = NEWID()
            INSERT  dbo.aspnet_Applications (ApplicationId, ApplicationName, LoweredApplicationName)
            VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName))
        END


        IF( @TranStarted = 1 )
        BEGIN
            IF(@@ERROR = 0)
            BEGIN
	        SET @TranStarted = 0
	        COMMIT TRANSACTION
            END
            ELSE
            BEGIN
                SET @TranStarted = 0
                ROLLBACK TRANSACTION
            END
        END
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Personalization_GetApplicationId]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Personalization_GetApplicationId] (
    @ApplicationName NVARCHAR(256),
    @ApplicationId UNIQUEIDENTIFIER OUT)
AS
BEGIN
    SELECT @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_RegisterSchemaVersion]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_RegisterSchemaVersion]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128),
    @IsCurrentVersion          bit,
    @RemoveIncompatibleSchema  bit
AS
BEGIN
    IF( @RemoveIncompatibleSchema = 1 )
    BEGIN
        DELETE FROM dbo.aspnet_SchemaVersions WHERE Feature = LOWER( @Feature )
    END
    ELSE
    BEGIN
        IF( @IsCurrentVersion = 1 )
        BEGIN
            UPDATE dbo.aspnet_SchemaVersions
            SET IsCurrentVersion = 0
            WHERE Feature = LOWER( @Feature )
        END
    END

    INSERT  dbo.aspnet_SchemaVersions( Feature, CompatibleSchemaVersion, IsCurrentVersion )
    VALUES( LOWER( @Feature ), @CompatibleSchemaVersion, @IsCurrentVersion )
END
GO
/****** Object:  StoredProcedure [dbo].[GetLabelsOnDemandCount]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 28 August 2017
-- Description:	Selects all LabelsOnDemand for a specific DateTime, Clinic, ModifiedBy
-- =============================================
CREATE PROCEDURE [dbo].[GetLabelsOnDemandCount]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT SiteCode, BatchDate, ModifiedBy, COUNT(OrderNumber) AS OrderCnt
	FROM dbo.LabelsOnDemand
	WHERE SiteCode = @SiteCode 
	GROUP BY SiteCode, ModifiedBy, BatchDate
END
GO
/****** Object:  StoredProcedure [dbo].[GetGlobalDefaults]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 20 June 2017
-- Description:	Retrieves all Global Defaults except Frame Item Defaults
-- =============================================
CREATE PROCEDURE [dbo].[GetGlobalDefaults]
	@Code VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM dbo.GlobalDefaults WHERE Code = @Code 
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameRxRestrictions]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10 January 2017
-- Description:	This stored procedure retrieves any 
--		Rx Restrictions for a specified FrameCode.
-- =============================================
CREATE PROCEDURE [dbo].[GetFrameRxRestrictions]
	-- Add the parameters for the stored procedure here
	@FrameCode VARCHAR(5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.RxFrameRestrictions
	WHERE FrameCode = @FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameLabRestrictions]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/14/15
-- Description:	Retrieves all lab restrictions for a
--		provided FrameCode
-- Modified:  1/6/17 - BAF - to include check for 
--		Clinic Region and FrameCode of 'SIDES'
-- =============================================
CREATE PROCEDURE [dbo].[GetFrameLabRestrictions]
	-- Add the parameters for the stored procedure here
	@FrameCode VARCHAR(15),
	@SiteCode VARCHAR(6)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	--SELECT f.*, fr.SiteCode FROM dbo.Frame f 
	--	INNER JOIN dbo.FrameRestrictions fr	ON f.FrameCode = fr.FrameCode
	--WHERE f.FrameCode = @FrameCode AND fr.SiteType = 'LAB'
	
	SELECT distinct f.*, Case when f.FrameCode = 'SIDES' and sc.Region IN (1,3) then lt.[Description]--'MBAMC1' 
		when f.FrameCode = 'SIDES' and sc.Region not IN (1,3) then lt.[Description]--'MNOST1'
		when f.FrameCode <> 'SIDES' then fr.SiteCode
	 end SiteCode
		FROM dbo.Frame f 
		INNER JOIN dbo.FrameRestrictions fr	ON f.FrameCode = fr.FrameCode
		Left Join dbo.SiteCode sc on sc.SiteCode = @SiteCode
		LEFT JOIN dbo.LookupTable lt ON lt.[Text] = sc.Region
	WHERE f.FrameCode = @FrameCode AND fr.SiteType = 'LAB'
		AND lt.[text] = sc.region AND lt.code = 'Region'
END
GO
/****** Object:  StoredProcedure [dbo].[GetLensStock]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 13 Feb 2017
-- Description:	Select the lens stock for a specific lab
-- Modified:  04/04/19 - baf - Added Capability
-- =============================================
CREATE PROCEDURE [dbo].[GetLensStock]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select ID, Material, Cylinder, MaxPlus, MaxMinus, IsStocked, Capability
	from dbo.LensStock where SiteCode = @SiteCode AND IsActive = 1
	Order by Material

END
GO
/****** Object:  StoredProcedure [dbo].[GetNextOrderNumberBySiteCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  3/19/2015 - BAF - Modified to split multiple pair orders into
--		single pair orders that LMS can utilize
-- =============================================
CREATE PROCEDURE [dbo].[GetNextOrderNumberBySiteCode] 
@ClinicSiteCode VARCHAR(6),
@RecCnt INT,
--@NextNumber int OUTPUT,
--@FiscalYear varchar(2) output
@OrderNumbers VARCHAR(MAX) = '' 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @AsOf DATETIME = GETDATE(),
		@Answer VARCHAR(4),
		@TestNumber INT,
		@Cntr INT = 1,
		@NextNumber INT,
		@FiscalYear VARCHAR(2)
	--IF(MONTH(@AsOf) > 10) 
	--	SET @Answer = CAST(YEAR(@AsOf) + 1 AS VARCHAR(4))
	--ELSE
	
	SET @Answer = CAST(YEAR(@AsOf) AS VARCHAR(4))
	SELECT @Answer = SUBSTRING(@Answer, 3, 2)
	
	SELECT @TestNumber = NextNumber FROM dbo.OrderCount WHERE SiteCode = @ClinicSiteCode AND FiscalYear = @Answer
	
	WHILE @Cntr <= @RecCnt
		BEGIN
			IF(@TestNumber >= 1)
			BEGIN
				SELECT @NextNumber = NextNumber, @FiscalYear = FiscalYear FROM dbo.OrderCount where SiteCode = @ClinicSiteCode
					and FiscalYear = @Answer
				
				UPDATE dbo.OrderCount
				SET PresentNumber = NextNumber
				WHERE SiteCode = @ClinicSiteCode AND FiscalYear = @Answer
			END
			ELSE
			BEGIN
				INSERT INTO dbo.OrderCount
						( SiteCode ,
						  FiscalYear ,
						  PresentNumber
						)
				VALUES  ( @ClinicSiteCode , -- SiteCode - varchar(6)
						  @Answer , -- FiscalYear - varchar(2)
						  0  -- PresentNumber - int
						)
				SELECT @NextNumber = NextNumber, @FiscalYear = FiscalYear FROM dbo.OrderCount
				WHERE SiteCode = @ClinicSiteCode AND FiscalYear = @Answer
				
				UPDATE dbo.OrderCount
				SET PresentNumber = NextNumber
				WHERE SiteCode = @ClinicSiteCode AND FiscalYear = @Answer
			END
			SET @Cntr = @Cntr + 1
			IF @OrderNumbers = ''
				SET @OrderNumbers = @ClinicSiteCode + RIGHT('0000000' + CAST(@NextNumber AS VARCHAR(7)),7)  + '-' + @Answer
			ELSE
				SET @OrderNumbers = @OrderNumbers + ',' + @ClinicSiteCode + RIGHT('0000000' + CAST(@NextNumber AS VARCHAR(7)),7) 
					+ '-' + @Answer
		END
		SELECT @OrderNumbers AS OrderNumber;
END
GO
/****** Object:  StoredProcedure [dbo].[GetLookupTypes]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLookupTypes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT(Code) 
	FROM dbo.LookupTable
END
GO
/****** Object:  StoredProcedure [dbo].[GetLookupsByType]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetLookupsByType] 
@LookupType		VARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Code, 
	[Description], 
	ID, 
	[Text], 
	Value, 
	IsActive, 
	ModifiedBy, 
	DateLastModified 
	FROM dbo.LookupTable
	WHERE Code = @LookupType
END
GO
/****** Object:  StoredProcedure [dbo].[GetChanges]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 4 November 2014
-- Description:	This Stored Procedure allows for the 
--	Retrieval of all changes that have taken place
--	since SRTSweb was put into production.		
-- =============================================
CREATE PROCEDURE [dbo].[GetChanges]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.ReleaseChanges
	ORDER BY VersionNbr
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameEligiblityParts]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetFrameEligiblityParts]
AS
Select distinct LEFT(EligibilityFrame,3) as Grade, SUBSTRING(EligibilityFrame,4,1) as BOS, 
	SUBSTRING(EligibilityFrame,5,2) as Status from dbo.FrameEligibility order by BOS, [Status], Grade
GO
/****** Object:  StoredProcedure [dbo].[GetFrameDefaults]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 29 Nov 2016
-- Description:	Retrieves defaults for a specified framecode
-- =============================================
CREATE PROCEDURE [dbo].[GetFrameDefaults]
	-- Add the parameters for the stored procedure here
	@FrameCode VarChar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from dbo.FrameDefaults 
	where FrameCode = @FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameItems]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 8/10/15
-- Description:	This stored procedure retrieves a 
--	distinct list of all available frame items
-- =============================================
CREATE PROCEDURE [dbo].[GetFrameItems]
	-- Add the parameters for the stored procedure here
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   -- Insert statements for procedure here
	SELECT DISTINCT TypeEntry, Value, [Text]
	FROM dbo.FrameItem 
	WHERE IsActive = 1 ORDER BY TypeEntry
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameItemEligibilityByFrameCode]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFrameItemEligibilityByFrameCode] 
@FrameCode	VARCHAR(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT EligibilityFrameItem FROM dbo.FrameItemEligibility
	WHERE FrameCode = @FrameCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMS_GetUserFacilities]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 7 May 14
-- Description:	This SP returns a list of SiteCodes for a Users Site, 
--				Multiple Primary/Secondary, and Single Primary/Secondary Sites.
-- =============================================
CREATE PROCEDURE [dbo].[GetCMS_GetUserFacilities]
@SiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select t.SiteName, t.SiteCode, t.IsActive, t.SiteType
	from SiteCode t
	where
	t.SiteCode = @SiteCode
	or t.MultiPrimary = @SiteCode
	or t.MultiSecondary = @SiteCode
	or t.SinglePrimary = @SiteCode
	or t.SingleSecondary = @SiteCode
	and t.IsActive = 1
	
	union

	select s.SiteName, s.SiteCode, s.IsActive, s.SiteType
	from SiteCode s
	where s.SiteCode = @SiteCode
	and s.IsActive = 1

	UNION

	select t.SiteName, t.SiteCode, t.IsActive, t.SiteType
	from SiteCode t
	where t.SiteCode = (
	select s.MultiPrimary
	from SiteCode s
	where s.SiteCode = @SiteCode
	)

	UNION

	select t.SiteName, t.SiteCode, t.IsActive, t.SiteType
	from SiteCode t
	where t.SiteCode = (
	select s.MultiSecondary
	from SiteCode s
	where s.SiteCode = @SiteCode
	)

	UNION

	select t.SiteName, t.SiteCode, t.IsActive, t.SiteType
	from SiteCode t
	where t.SiteCode = (
	select s.SinglePrimary
	from SiteCode s
	where s.SiteCode = @SiteCode
	)

	UNION

	select t.SiteName, t.SiteCode, t.IsActive, t.SiteType
	from SiteCode t
	where t.SiteCode = (
	select s.SingleSecondary
	from SiteCode s
	where s.SiteCode = @SiteCode
	)
	order by t.SiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMS_ContentTypes]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMS_ContentTypes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT cmsContentTypeID, cmsContentTypeName, cmsContentDescription 
    FROM dbo.CMS_ContentType
    WHERE isActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMS_RecipientTypes]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMS_RecipientTypes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT c.cmsRecipientTypeID, c.cmsRecipientTypeName, c.cmsRecipientTypeDescription 
    FROM dbo.CMS_RecipientType c
    WHERE c.isActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMS_RecipientGroupTypes]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMS_RecipientGroupTypes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT cmsRecipientGroupID, cmsRecipientGroupName, cmsRecipientGroupDescription FROM dbo.CMS_RecipientGroup
END
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [ELMAH_Error]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            @TimeUtc
        )
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS

    SET NOCOUNT ON

    SELECT 
        [AllXml]
    FROM 
        [ELMAH_Error]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS 

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT 
        @TotalCount = COUNT(1) 
    FROM 
        [ELMAH_Error]
    WHERE 
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT  
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM 
            [ELMAH_Error]
        WHERE   
            [Application] = @Application
        ORDER BY 
            [TimeUtc] DESC, 
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT 
        errorId     = [ErrorId], 
        application = [Application],
        host        = [Host], 
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode], 
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM 
        [ELMAH_Error] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND 
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC, 
        [Sequence] DESC
    FOR
        XML AUTO
GO
/****** Object:  StoredProcedure [dbo].[GetAllLookups]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllLookups] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Code, 
	[Description], 
	ID, 
	[Text], 
	Value, 
	IsActive, 
	ModifiedBy, 
	DateLastModified 
	FROM dbo.LookupTable
	WHERE IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllFrameswithPreferences]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 7/17/2017
-- Description:	Retrieve all frames that could have preferences attached.
-- =============================================
CREATE PROCEDURE [dbo].[GetAllFrameswithPreferences]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT FrameCode, FrameDescription, FrameCode + ' - ' + FrameDescription as FrameLongDescription, FrameNotes, IsInsert,
		MaxPair, ImageURL, IsActive, IsFOC
    FROM dbo.Frame
    WHERE IsActive = 1
		AND IsInsert = 0 
		AND (FrameDescription NOT LIKE ('%MASK') OR FrameDescription NOT LIKE ('PMI'))
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllFrames]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  7/17/2017 - baf - Modified to only pull active frames
-- =============================================
CREATE PROCEDURE [dbo].[GetAllFrames] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT FrameCode, FrameDescription, FrameCode + ' - ' + FrameDescription as FrameLongDescription, FrameNotes, IsInsert,
		MaxPair, ImageURL, IsActive, IsFOC
    FROM dbo.Frame
    WHERE IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllSiteCodes]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllSiteCodes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT SiteCode
	from dbo.SiteCode
	ORDER BY SiteCode ASC
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnWrkDays]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 24 July 2013
-- Description:	This function returns the number of workdays 
--		between two dates, weekends and holidays are excluded.
-- =============================================
CREATE FUNCTION [dbo].[fnWrkDays]
(
	-- Add the parameters for the function here
	@FromDate Date, 
	@ToDate Date
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @wrkDays int

	-- Add the T-SQL statements to compute the return value here
	SELECT  @WrkDays = (DATEDIFF(dd,@FromDate,@ToDate) - 
		(2*DateDiff(ww,@FromDate,@ToDate)) -
		(Select count(*) from dbo.LookupTable where Code = 'Holiday' 
		and CAST(Text as Date) between @FromDate and @ToDate)) 
	

	-- Return the result of the function
	RETURN @wrkDays

END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_AddBizDays]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_AddBizDays]
--Accepts Date ‘X’ and Integer ‘Y’

(
@dtBeginDate DATETIME,
@iBizDays INT
)

RETURNS DATETIME
--Result is Date ‘Z’ that is ‘Y’ business days after supplied Date ‘X’

AS

BEGIN


DECLARE @dtEndDate DATETIME
DECLARE @dtDateHolder DATETIME
DECLARE @dtNextHol DATETIME

SET @dtDateHolder = @dtBeginDate
SET @dtEndDate = DATEADD(d,@iBizDays,@dtBeginDate)
--Find the first Public Holiday Date
SELECT @dtNextHol = MIN(CAST(text AS DATE)) FROM dbo.LookupTable WHERE code = 'Holiday' AND CAST(text AS DATE) >= @dtDateHolder

WHILE (@dtDateHolder <= @dtEndDate)
BEGIN

--Is the date being checked a Saturday or Sunday?
IF((DATEPART(dw, @dtDateHolder) IN (1, 7))
--Is the date being checked a holiday?
OR (DATEADD(dd, 0, DATEDIFF(dd, 0, @dtDateHolder))=@dtNextHol))
--NOTE: DATEDIFF trick used above to discard TIMESTAMP

BEGIN

-- Extend the date range for Weekends and Holidays by one day
SET @dtEndDate = DATEADD(d,1,@dtEndDate)
IF (DATEADD(dd, 0, DATEDIFF(dd, 0, @dtDateHolder))=@dtNextHol)
BEGIN
-- get the next  public holiday date
SELECT @dtNextHol = MIN(CAST(text AS DATE)) FROM dbo.LookupTable WHERE code = 'Holiday' AND CAST(text AS DATE) > @dtDateHolder
END
END

--Move to the next day in the range and loop back to check it
SET @dtDateHolder = DATEADD(d,1,@dtDateHolder)

END

--Respond with newly determined end date
RETURN @dtEndDate

END
GO
/****** Object:  StoredProcedure [dbo].[GetRulesOfBehavior]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 7 June 2019
-- Description:	Retrieves the SRTS Rules Of Behavior
-- =============================================
CREATE PROCEDURE [dbo].[GetRulesOfBehavior]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from dbo.RulesOfBehavior Order by ID Asc
END
GO
/****** Object:  StoredProcedure [dbo].[GetSiteOnlyBySiteID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSiteOnlyBySiteID] 
	-- Add the parameters for the stored procedure here
	@SiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.SiteName
		,a.SiteType
		,a.SiteDescription
		,a.BOS
		,a.IsMultivision
		,a.EMailAddress
		,a.DSNPhoneNumber
		,a.RegPhoneNumber
		,a.IsAPOCompatible
		,a.MaxEyeSize
		,a.MaxFramesPerMonth
		,a.MaxPower
		,a.HasLMS
		,a.Region
		,a.MultiPrimary
		,a.MultiSecondary
		,a.SinglePrimary
		,a.SingleSecondary
		,a.ShipToPatientLab  -- Added 9/17/14 BAF for Lab Mail to Patient
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
		,a.IsReimbursable
	from dbo.SiteCode a
	where a.SiteCode = @SiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameDefaults]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 Dec 2016
-- Description:	Use to Insert Frame Defaults based upon a
--			specific frame.
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameDefaults]
	@FrameCode VarChar(10),
	@EyeSize VarChar(10),
	@BridgeSize VarChar(10),
	@Temple VarChar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Insert into dbo.FrameDefaults	
    	Values (@FrameCode, @EyeSize, @BridgeSize, @Temple)
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameItem]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameItem] 
@Value				varchar(15),
@TypeEntry			varchar(50),
@Text				varchar(100),
@ModifiedBy			varchar(200),
@FrameCode			VARCHAR(15),
@EligibilityCode	VARCHAR(8)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT [Value] FROM dbo.FrameItem
	WHERE Value = @Value AND TypeEntry = @TypeEntry)
	BEGIN
				INSERT INTO dbo.FrameItem
				([Value], TypeEntry, [Text], IsActive, ModifiedBy, DateLastModified)
				VALUES(@Value, @TypeEntry, @Text, 1, @ModifiedBy, GETDATE())
	END

	IF NOT EXISTS
	(SELECT [Value] FROM dbo.EligibilityFrameItemsUnion
	WHERE [Value] = @Value AND TypeEntry = @TypeEntry AND EligibilityCode = @EligibilityCode)
	BEGIN
		INSERT INTO dbo.EligibilityFrameItemsUnion
		(FrameCode, EligibilityCode, [Value], TypeEntry)
		VALUES(@FrameCode, @EligibilityCode, @Value, @TypeEntry)
	END
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertLookupTable]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertLookupTable]
@Code		VARCHAR(20),
@Text		varchar(200),
@Value		varchar(10),
@Description	varchar(200),
@IsActive		bit,
@ModifiedBy		varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    insert into dbo.LookupTable
    (Code, [Description], [Text], Value, IsActive, ModifiedBy, DateLastModified)
    VALUES(@Code, @Description, @Text, @Value, @IsActive, @ModifiedBy, GETDATE())
		
		select * from dbo.LookupTable
END
GO
/****** Object:  StoredProcedure [dbo].[GetReOrderReasons]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 25 July 2016
-- Description:	Purpose is to return a list of ReOrder Reason Comments
-- =============================================
CREATE PROCEDURE [dbo].[GetReOrderReasons]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.ROComments
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetReleaseChanges]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 14 May 2015
-- Description:	Retrieve version change information
--		If no version number, pulls all
-- =============================================
CREATE PROCEDURE [dbo].[GetReleaseChanges]
	-- Add the parameters for the stored procedure here
	@Version VarChar(10) = Null
	
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select * from dbo.ReleaseChanges
	where @Version is null or VersionNbr = @Version 
END
GO
/****** Object:  StoredProcedure [dbo].[GetTheaterLocationCodes]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetTheaterLocationCodes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DateLastModified, 
	IsActive, 
	ModifiedBy, 
	TheaterCode,
	[Description]
	FROM dbo.TheaterLocationCode 
	WHERE IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetTempSiteBySiteID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetTempSiteBySiteID] 
	-- Add the parameters for the stored procedure here
	@SiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.SiteName
		,a.SiteType
		,a.SiteDescription
		,a.BOS
		,a.IsMultivision
		,a.EMailAddress
		,a.DSNPhoneNumber
		,a.RegPhoneNumber
		,a.IsAPOCompatible
		,a.MaxEyeSize
		,a.MaxFramesPerMonth
		,a.MaxPower
		,a.HasLMS
		,a.Region
		,a.MultiPrimary
		,a.MultiSecondary
		,a.SinglePrimary
		,a.SingleSecondary
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
	from dbo.SiteCode a
	where a.SiteCode = @SiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSQLErrorLog]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertSQLErrorLog] 
@ErrorLogID	[int] = 0 OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Output parameter value of 0 indicates that error 
    -- information was not logged
    SET @ErrorLogID = 0;

    BEGIN TRY
        -- Return if there is no error information to log.
        IF ERROR_NUMBER() IS NULL
            RETURN;

        -- Return if inside an uncommittable transaction.
        -- Data insertion/modification is not allowed when 
        -- a transaction is in an uncommittable state.
        IF XACT_STATE() = -1
        BEGIN
            RETURN
        END;

        INSERT [dbo].[SQLErrorLog] 
            (
            [ModifiedBy], 
            [ErrorNumber], 
            [ErrorSeverity], 
            [ErrorState], 
            [ErrorProcedure], 
            [ErrorLine], 
            [ErrorMessage],
            [ErrorDate]
            ) 
        VALUES 
            (
            CONVERT(sysname, CURRENT_USER), 
            ERROR_NUMBER(),
            ERROR_SEVERITY(),
            ERROR_STATE(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
            );

        -- Pass back the ErrorLogID of the row inserted
        SELECT @ErrorLogID = @@IDENTITY
    END TRY
    BEGIN CATCH
        RETURN -1
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertAuditTrail]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10/5/2014
-- Description:	Write new records to the Audit_Trail table
-- =============================================
CREATE PROCEDURE [dbo].[InsertAuditTrail]
	-- Add the parameters for the stored procedure here
	@ChangeDate datetime,
	@FieldName VarChar(100),
	@FieldValue VarChar(MAX),
	@RecordID VarChar(50),  -- Identifier for Record inserted, Patient Record ID, Exam Record ID, etc.
	@AULID int , -- Audit Lookup Table ID
	@ModifiedBy VarChar (200), -- Who added the record
	@Action Char(1) -- Audit Action Code
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	Declare @FVal varchar(Max)
	
	IF @FieldValue is null 
		Begin
			Set @FVal = ''
		End
	ELSE
		Begin
			Set @FVal = @FieldValue		
		End
    -- Insert statements for procedure here
	Insert into dbo.Audit_Trail (ChangeDate,FieldName, FieldValue, RecordID, AULID, ModifiedBy, [Action])
	Values (@ChangeDate, @FieldName, @Fval, @RecordID, @AULID, @ModifiedBy, @Action)
END
GO
/****** Object:  StoredProcedure [dbo].[InsertAuditSingleRead]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10/14/14
-- Description:	Auditing trial of single record 
--		being read and who they were read by.
-- =============================================
Create PROCEDURE [dbo].[InsertAuditSingleRead]
	-- Add the parameters for the stored procedure here
	@ReadDate DateTime,
	@PatientID int,
	@ALUID int,
	@Reader VarChar(200),
	@ReadRecID VarChar(50)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Insert into dbo.Audit_SingleRead (DateRead, PatientID, ALUID, Reader, ReadRecID)
	Values (@ReadDate, @PatientID, @ALUID, @Reader, @ReadRecID)
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertAuditMultiRead]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10/15/14
-- Description:	Add Auditing record for a stored
--		procedure that pulls multiple records.
-- =============================================
CREATE PROCEDURE [dbo].[InsertAuditMultiRead]
	-- Add the parameters for the stored procedure here
	@ReadDate DateTime,
	@Reader VarChar(200),
	@AULID int,
	@PatientList VarChar(Max),
	@Notes VarChar(Max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Insert into dbo.Audit_MultiRead (ReadDate, Reader, AULID, PatientIDList, Notes)
	Values (@ReadDate, @Reader, @AULID, @PatientList, @Notes)
END
GO
/****** Object:  StoredProcedure [dbo].[InsertReleaseChanges]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 14 May 2015
-- Description:	Insert release notes
-- =============================================
CREATE PROCEDURE [dbo].[InsertReleaseChanges]
	-- Add the parameters for the stored procedure here
	@Version VarChar(10),
	@VersionDate DateTime,
	@ChangesMade VarChar(Max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Insert into dbo.ReleaseChanges 
	Values (@Version, @VersionDate, @ChangesMade)
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameRxRestrictions]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10 Jan 2017
-- Description:	Insert Frame and Rx Restrictions 
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameRxRestrictions]
	-- Add the parameters for the stored procedure here
		@FrameCode VARCHAR(5),
		@MaxSphere NUMERIC(6,2) = Null,
		@MinSphere NUMERIC(6,2) = Null,
		@MaxCylinder NUMERIC(6,2) = Null,
		@MinCylinder NUMERIC(6,2) = Null,
		@Material VARCHAR(4)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.RxFrameRestrictions
	VALUES (@FrameCode, @MaxSphere, @MinSphere, @MaxCylinder, @MinCylinder, @Material)
	
END
GO
/****** Object:  StoredProcedure [dbo].[TVP_Eligible]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TVP_Eligible]
/*
	This stored procedure allows for inserting records into
	The User-Defined Table TVP_FrameEligibility
	Created:  8/14/15 - baf
	Modified:
	
*/
 @TVP as TVP_FrameEligibility READONLY
 
 AS
 BEGIN
	INSERT INTO dbo.TVP_FrameEligibility
	        ( FrameCode, Eligibility )
	VALUES  ( '', -- FrameCode - varchar(15)
	          ''  -- Eligibility - varchar(8)
	          )
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateAuthorizationSiteCodeByUserName]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 9 Feb 15
-- Description:	Updates the sitecode based on the UserName.  It is possible that there will be multiple updates.
--				This is currently being used when an Individuals sitecode is updated.
-- =============================================
CREATE PROCEDURE [dbo].[UpdateAuthorizationSiteCodeByUserName]
	-- Add the parameters for the stored procedure here
	@SiteCode varchar(6),
	@UserName varchar(256)
AS
BEGIN
	SET NOCOUNT ON;

    UPDATE [Authorization]
    SET SiteCode = @SiteCode
    WHERE LOWER(UserName) = LOWER(@UserName);
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateFrameDefaults]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 Dec 2016
-- Description:	Use to update Frame Defaults based upon a
--			specific frame.
-- =============================================
CREATE PROCEDURE [dbo].[UpdateFrameDefaults]
	@FrameCode VarChar(10),
	@EyeSize VarChar(10) ,
	@BridgeSize VarChar(10),
	@Temple VarChar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update dbo.FrameDefaults
	set EyeSize = @EyeSize, BridgeSize = @BridgeSize,
		Temple = @Temple
	where FrameCode = @FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateROComments]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 1 August 2016
-- Description:	This SP allows for the updating of a ReOrder Reason
-- =============================================
CREATE PROCEDURE [dbo].[UpdateROComments]
	-- Add the parameters for the stored procedure here
	@ID INT, -- ReORder Comment ID,
	@ROComment VARCHAR(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.ROComments SET ROReason = @ROComment WHERE ID = @ID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateLookupTable]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateLookupTable]
@ID			int, 
@Code		VARCHAR(20),
@Text		varchar(200),
@Value		varchar(10),
@Description	varchar(200),
@IsActive		bit,
@ModifiedBy		varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    update dbo.LookupTable
    set
		Code = @Code, 
		[Description] = @Description, 
		[Text] = @Text, 
		Value = @Value, 
		IsActive = @IsActive, 
		ModifiedBy = @ModifiedBy, 
		DateLastModified = GETDATE() 
		WHERE ID = @ID
		
		select * from dbo.LookupTable
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateSiteAddress]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- MOdified:  9/10/15 - baf - To save old record information
-- =============================================
CREATE PROCEDURE [dbo].[UpdateSiteAddress] 
@Address1	varchar(200),
@Address2	varchar(200),
@Address3	varchar(200),
@City		varchar(200),
@State		varchar(2),
@Country	varchar(2),
@ZipCode	varchar(10),
@IsConus	bit,
@AddressType	varchar(4),
@IsActive	bit,
@ModifiedBy		VARCHAR(200),
@SiteCode	varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT 
		
	BEGIN TRY
		BEGIN TRANSACTION
			-- Save old SiteAddress info
			Insert into dbo.SiteAddress_ChangedInfo (SiteCode, Address1, Address2, Address3, AddressType, City, Country, State, ZipCode,
				IsConus, IsActive, ModifiedBy, DateLastModified, LegacyID,DateChanged, ChangedBy)
			Select SiteCode, Address1, Address2, Address3, AddressType, City, Country, State, ZipCode,
				IsConus, IsActive, ModifiedBy, DateLastModified, LegacyID, GETDATE(), @ModifiedBy
			from dbo.SiteAddress where SiteCode = @SiteCode and AddressType = @AddressType
		
			-- Update SiteAddress with new info
				Update dbo.SiteAddress
				SET Address1 = @Address1
				, Address2 = @Address2
				, Address3 = @Address3
				, City = @City
				, [State] = @State
				, Country = @Country
				, ZipCode = @ZipCode
				, IsConus = @IsConus
				, IsActive = @IsActive
				, ModifiedBy = @ModifiedBy
				, DateLastModified = GETDATE()
				WHERE SiteCode = @SiteCode and AddressType = @AddressType

		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
	
		SELECT SiteCode,
		Address1,
		Address2,
		Address3,
		City,
		[State],
		Country,
		ZipCode,
		IsConus,
		AddressType,
		IsActive,
		ModifiedBy,
		DateLastModified
		FROM dbo.SiteAddress
		WHERE SiteCode = @SiteCode AND AddressType = @AddressType
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateSite]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateSite] 
@SiteCode	varchar(6),
@SiteName	varchar(50),
@SiteType	varchar(50),
@SiteDescription	varchar(100),
@BOS		varchar(1),
@IsMultivision	bit,
@EMailAddress	varchar(512),
@DSNPhoneNumber	varchar(20),
@RegPhoneNumber	varchar(20),
@IsAPOCompatible	bit,
@MaxEyeSize		int,
@MaxFramesPerMonth int,
@MaxPower	numeric(18,0),
@HasLMS		bit,
@Region		int,
@MultiPrimary	varchar(6),
@MultiSecondary	varchar(6),
@SinglePrimary	varchar(6),
@SingleSecondary	varchar(6),
@ShipToPatientLab	bit,		--Added 9/17/14 BAF
@IsActive	bit,
@ModifiedBy		VARCHAR(200),
@IsReimbursable bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT, @Reg int = 0
	
	IF @Region = 0
	Begin
		Select @Reg = Region from dbo.SiteCode where SiteCode= @SiteCode
	End 
	Else
	BEGIN
		Set @Reg = @Region
	END
		
	BEGIN TRY
		BEGIN TRANSACTION
		
			--Insert old record into SiteCode_ChangedInfo
			Insert into dbo.SiteCode_ChangedInfo (SiteCode, SiteName, SiteType, SiteDescription, BOS, IsMultivision, EMailAddress, DSNPhoneNumber, 
				RegPhoneNumber, IsAPOCompatible, MaxEyeSize, MaxFramesPerMonth, MaxPower, HasLMS, Region, MultiPrimary, MultiSecondary, 
				SinglePrimary, SingleSecondary, IsActive, ShipToPatientLab, ModifiedBy, DateLastModified, IsReimbursable, LegacyID, DateChanged,
				ChangedBy)
			Select SiteCode, SiteName, SiteType, SiteDescription, BOS, IsMultivision, EMailAddress, DSNPhoneNumber, 
				RegPhoneNumber, IsAPOCompatible, MaxEyeSize, MaxFramesPerMonth, MaxPower, HasLMS, Region, MultiPrimary, MultiSecondary, 
				SinglePrimary, SingleSecondary, IsActive, ShipToPatientLab, ModifiedBy, DateLastModified, IsReimbursable, LegacyID,
				GETDATE(),@ModifiedBy
			from dbo.SiteCode where SiteCode = @SiteCode
							
			-- Update Record with New Information
				Update dbo.SiteCode
				SET SiteName = @SiteName
				, SiteType = @SiteType
				, SiteDescription = @SiteDescription
				, BOS = @BOS
				, IsMultivision = @IsMultivision
				, EMailAddress = @EMailAddress
				, DSNPhoneNumber = @DSNPhoneNumber
				, RegPhoneNumber = @RegPhoneNumber
				, IsAPOCompatible = @IsAPOCompatible
				, MaxEyeSize = @MaxEyeSize
				, MaxFramesPerMonth = @MaxFramesPerMonth
				, MaxPower = @MaxPower
				, HasLMS = @HasLMS
				,Region = @Reg
				,MultiPrimary=@MultiPrimary
				,MultiSecondary=@MultiSecondary
				,SinglePrimary=@SinglePrimary
				,SingleSecondary=@SingleSecondary
				, ShipToPatientLab = @ShipToPatientLab
				, IsActive = @IsActive
				, ModifiedBy = @ModifiedBy
				, DateLastModified = GETDATE()
				, IsReimbursable = @IsReimbursable
				WHERE SiteCode = @SiteCode

		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
	
		SELECT SiteCode,
		SiteName,
		SiteType,
		SiteDescription,
		BOS,
		IsMultivision,
		EMailAddress,
		DSNPhoneNumber,
		RegPhoneNumber,
		IsAPOCompatible,
		MaxEyeSize,
		MaxFramesPerMonth,
		MaxPower,
		HasLMS,
		Region,
		MultiPrimary,
		MultiSecondary,
		SinglePrimary,
		SingleSecondary,
		ShipToPatientLab,
		IsActive,
		IsReimbursable,
		ModifiedBy,
		DateLastModified
		FROM dbo.SiteCode
		WHERE SiteCode = @SiteCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateFrameEligibility]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE 	[dbo].[UpdateFrameEligibility]
	@EligiblityFrame VARCHAR(8),
	@IsActive BIT ,
	@ModifiedBy VARCHAR(200),
	@DateLastModified DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT = 0

		BEGIN TRY
			BEGIN TRANSACTION
				UPDATE dbo.FrameEligibility SET EligibilityFrame = @EligiblityFrame, IsActive = @IsActive,
					ModifiedBy = @ModifiedBy, DateLastModified = GETDATE()
				WHERE EligibilityFrame = @EligiblityFrame
			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[TVP_Insert]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TVP_Insert]
/*
	This stored procedure allows for inserting records into
	FrameITems and Frame ITem Union tables
	Created:  8/14/2015 - baf
	Modified:
	
*/
 @TVP as TVP_FrameItemsAndEligibility READONLY

AS
BEGIN
	SET NOCOUNT ON;
		
	-- Insert new FrameItems
		IF NOT EXISTS
		(SELECT [Value] FROM dbo.FrameItem
		WHERE Value IN (SELECT Value FROM @TVP) AND TypeEntry IN (SELECT TypeEntry FROM @TVP))
		BEGIN
			INSERT INTO dbo.FrameItem (Value ,TypeEntry ,Text ,IsActive ,ModifiedBy ,DateLastModified ,Availability)
			SELECT Value, TypeEntry, [Text], IsActive, ModifiedBy, GETDATE(), Availability FROM @TVP
		END
	
	
	--Insert new FrameItem Eligiblity
	IF NOT EXISTS 
		(SELECT EligibilityFrameItem FROM dbo.FrameItemEligibility
			WHERE EligibilityFrameItem IN (SELECT EligibilityCode FROM @TVP) AND FrameCode IN (SELECT FrameCode FROM @TVP))
		Begin
			INSERT INTO dbo.FrameItemEligibility (EligibilityFrameItem,FrameCode)
			SELECT DISTINCT EligibilityCode, FrameCode FROM @TVP
		End
		
	-- Insert FrameItemEligibiltyFrameItemUnion
	IF NOT EXISTS 
		(SELECT EligibiityFrameItem FROM dbo.FrameItemEligibilityFrameItemUnion
			WHERE EligibiityFrameItem IN (SELECT EligibilityCode FROM @TVP) AND FrameCode IN (SELECT FrameCode FROM @TVP)
				AND Value IN (SELECT Value FROM @TVP) AND TypeEntry IN (SELECT TypeEntry FROM @TVP))
		Begin
			INSERT INTO dbo.FrameItemEligibilityFrameItemUnion ( EligibiityFrameItem ,FrameCode ,Value ,TypeEntry)
			SELECT EligibilityCode, FrameCode, Value, TypeEntry FROM @TVP
		END

END
GO
/****** Object:  StoredProcedure [dbo].[UpdateLensStock]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 13 Feb 2017
-- Description:	Update a Labs Lens Stock
-- Modified By: 04/04/19 - baf Added Capability
-- =============================================
CREATE PROCEDURE [dbo].[UpdateLensStock]
	-- Add the parameters for the stored procedure here
	@ID int,
	@Material VarChar(5),
	@Cylinder Decimal (4,2),
	@MaxPlus Decimal (4,2),
	@MaxMinus Decimal (4,2),
	@IsStocked Bit,
	@ModifiedBy VarChar(200),
	@Success INT OUTPUT,
	@Capability Char(2)
	
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID	INT = 0;

	BEGIN TRY
		BEGIN TRANSACTION
			Update dbo.LensStock Set Material = @Material,
				Cylinder = @Cylinder, MaxPlus = @MaxPlus, 
				MaxMinus = @MaxMinus, IsStocked = @IsStocked,
				Capability = @Capability,
				DateLastModified = GETDATE(), ModifiedBy = @ModifiedBy
			Where ID = @ID

			Set @Success = 1	
		COMMIT TRANSACTION
	
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		Set @Success = 0
		Return
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateNextFOCDate]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
Author:		Barb Fieldhausen
Create date: 14 Dec 2015
Description:	Updates an Individuals NextFOC Date
Modified:
===============================================*/
CREATE PROCEDURE [dbo].[UpdateNextFOCDate]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.Individual SET NextFOCDate = DATEADD(YEAR,1,GETDATE()), DateLastModified = GETDATE() WHERE ID = @ID
END
GO
/****** Object:  StoredProcedure [dbo].[SyncUserToIndividual]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 16 May 14
-- Description:	This stored proceduer updates the individual table with a users aspnet_UserId.
-- Modification: 18 Nov 2014 - Added a parameter to determine if the sync is to be removed or added.
-- =============================================
CREATE PROCEDURE [dbo].[SyncUserToIndividual]
	@aspnetUserName varchar(256),
	@individualId int,
	@doRemove bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

	IF @doRemove = 0
	BEGIN
		UPDATE dbo.Individual
		SET aspnet_UserID = (SELECT UserId FROM aspnet_Users WHERE UserName = @aspnetUserName)
		WHERE ID = @individualId;
	END
	ELSE
	BEGIN
		UPDATE dbo.Individual
		SET aspnet_UserID = null
		WHERE ID = @individualId;	
	END
END
GO
/****** Object:  StoredProcedure [dbo].[InsertTheaterLocationCode]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		Barb Fieldhausen
 Create date: 8/18/2017
 Description:	Allows for inserting or updating a 
	TheaterLocationCode
 Modified:
 =============================================*/
CREATE PROCEDURE [dbo].[InsertTheaterLocationCode]
	-- Add the parameters for the stored procedure here
	@TheaterLocationCode VARCHAR(9),
	@IsActive BIT = 0,
	@ModifiedBy VARCHAR(200),
	@DateLastModified DATETIME,
	@LegacyID INT = 0,
	@Desription VARCHAR(75),
	@Success INT OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT = 0
	
	DECLARE @TheaterCode VARCHAR(9) = NULL
	
	Select @TheaterCode = TheaterCode FROM dbo.TheaterLocationCode WHERE TheaterCode = @TheaterLocationCode

		BEGIN TRY
			IF @TheaterCode IS NULL -- Insert new record
				Begin
				BEGIN TRANSACTION
					INSERT INTO dbo.TheaterLocationCode (TheaterCode, IsActive, ModifiedBy, DateLastModified, LegacyID, [Description])
					VALUES  (@TheaterLocationCode,@IsActive, @ModifiedBy,@DateLastModified, 0,@Desription)
				COMMIT TRANSACTION
				END
			ELSE -- Update exiting Record
				BEGIN
				BEGIN TRANSACTION
					UPDATE dbo.TheaterLocationCode SET IsActive = @IsActive, ModifiedBy = @ModifiedBy,
						DateLastModified = @DateLastModified, [Description] = @Desription
					WHERE TheaterCode = @TheaterLocationCode
				COMMIT TRANSACTION
				END
			SET @Success = 1
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END
			SET @Success = 0
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateFrame]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 8/18/2015
-- Description:	Allows for updating a frame record
-- Modified:
-- =============================================
CREATE PROCEDURE [dbo].[UpdateFrame]
	-- Add the parameters for the stored procedure here
	@FrameCode VARCHAR(15),
	@FrameDescription VARCHAR(100) = '',
	@FrameNotes VARCHAR(100) = '',
	@IsInsert BIT = 0,
	@MaxPair INT = 0,
	@ImageURL VARCHAR(250) = '',
	@IsActive BIT = 0,
	@ModifiedBy VARCHAR(200),
	@DateLastModified DATETIME,
	@IsFOC BIT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT = 0

		BEGIN TRY
			BEGIN TRANSACTION
				UPDATE dbo.Frame SET FrameDescription = @FrameDescription, FrameNotes = @FrameNotes,
					IsInsert = @IsInsert, MaxPair = @MaxPair, ImageURL = @ImageURL, IsActive = @IsActive,
					ModifiedBy = @ModifiedBy, DateLastModified = GETDATE(), IsFOC = @IsFOC
				WHERE FrameCode = @FrameCode
			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameItemEligibilityUnion]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameItemEligibilityUnion] 
@EligibilityCode	varchar(8),
@FrameCode			varchar(15),
@Value				varchar(15),
@TypeEntry			varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT FrameCode FROM dbo.EligibilityFrameItemsUnion
	WHERE FrameCode = @FrameCode AND EligibilityCode = @EligibilityCode AND [Value] = @Value AND TypeEntry = @TypeEntry)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.EligibilityFrameItemsUnion
				(FrameCode, EligibilityCode, [Value], TypeEntry)
				VALUES(@FrameCode, @EligibilityCode, @Value, @TypeEntry)
			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
		   IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	SELECT FrameCode FROM dbo.EligibilityFrameItemsUnion
	WHERE FrameCode = @FrameCode AND EligibilityCode = @EligibilityCode AND [Value] = @Value AND TypeEntry = @TypeEntry

END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameItemEligibilityAndUnion]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameItemEligibilityAndUnion] 
@Value				varchar(15),
@TypeEntry			varchar(50),
@Text				varchar(100),
@ModifiedBy			varchar(200),
@FrameCode			VARCHAR(15),
@EligibilityCode	VARCHAR(8)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT [Value] FROM dbo.FrameItem
	WHERE Value = @Value AND TypeEntry = @TypeEntry)
	BEGIN
		INSERT INTO dbo.FrameItem
		([Value], TypeEntry, [Text], IsActive, ModifiedBy, DateLastModified)
		VALUES(@Value, @TypeEntry, @Text, 1, @ModifiedBy, GETDATE())
	END
	
	IF NOT EXISTS
	(SELECT EligibilityFrameItem FROM dbo.FrameItemEligibility
	WHERE EligibilityFrameItem = @EligibilityCode AND FrameCode = @FrameCode)
	BEGIN
		INSERT INTO dbo.FrameItemEligibility
		        ( EligibilityFrameItem, FrameCode )
		VALUES  ( @EligibilityCode, -- EligibilityFrameItem - varchar(2)
		          @FrameCode  -- FrameCode - varchar(15)
		          )
	END

	IF NOT EXISTS
	(SELECT [Value] FROM dbo.FrameItemEligibilityFrameItemUnion
	WHERE [Value] = @Value AND TypeEntry = @TypeEntry AND EligibiityFrameItem = @EligibilityCode AND FrameCode = @FrameCode)
	BEGIN
		INSERT INTO dbo.FrameItemEligibilityFrameItemUnion
		        ( EligibiityFrameItem ,
		          FrameCode ,
		          Value ,
		          TypeEntry
		        )
		VALUES  ( @EligibilityCode , -- EligibiityFrameItem - varchar(2)
		          @FrameCode , -- FrameCode - varchar(15)
		          @Value , -- Value - varchar(40)
		          @TypeEntry  -- TypeEntry - varchar(50)
		        )
	END
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertReorderComment]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 26 July 2016
-- Description:	This SP is used to isnert a new record into the ReOrder Comments Table
-- =============================================
CREATE PROCEDURE [dbo].[InsertReorderComment]
	-- Add the parameters for the stored procedure here
	@ROReason VarChar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @ErrorLogID	INT = 0
	
	BEGIN
		TRY
			INSERT INTO dbo.ROComments ( ROReason )
			VALUES  (@ROReason)
		END TRY
	BEGIN CATCH
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[IUSitePref_LabMailToPatient]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhasen
-- Create date: 18 July 2018
-- Description:	Use to Insert/Update Lab MailToPatient Preferences
-- Modified:  
-- =============================================
CREATE PROCEDURE [dbo].[IUSitePref_LabMailToPatient]
	-- Add the parameters for the stored procedure here
	@LabSiteCode VarChar(6),
	@LMTP BIT = Null,
	@LMTP_All Bit = Null,
	@CapacityNbr INT = Null,
	@Comments VarChar(500) = '',
	@ModifiedBy VarChar(200),
	@DateLastModified DateTime,
	@StartDate DateTime,
	@StopDate DateTime,
	@AnticipatedEndDate DateTime,
	@StatusReason VarChar(250) = '',
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_LabMailToPatient
	WHERE LabSiteCode = @LabSiteCode

	BEGIN TRY
		IF @RecCnt = 0
		BEGIN
			BEGIN TRANSACTION
				INSERT INTO dbo.SitePref_LabMailToPatient(LabSiteCode, LabMailToPatient, LMTP_AllClinics, OrderCapacity,
					Comments, ModifiedBy, DateLastModified, StartDate, StopDate, AnticipatedEndDate, StatusReason)
				VALUES (@LabSiteCode, @LMTP, @LMTP_All, @CapacityNbr, @Comments, @ModifiedBy, GETDATE(),@StartDate, @StopDate, 
					@AnticipatedEndDate, @StatusReason)
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			BEGIN TRANSACTION
				If @LMTP is Null
					Begin
						Select @LMTP = LabMailtoPatient from dbo.SitePref_LabMailToPatient where LabSiteCode = @LabSiteCode
					End

				If @LMTP_All is Null
					Begin
						Select @LMTP_All = LMTP_AllClinics from dbo.SitePref_LabMailToPatient where LabSiteCode = @LabSiteCode
					End

				If @CapacityNbr is Null
					Begin
						Select @CapacityNbr = OrderCapacity from dbo.SitePref_LabMailToPatient where LabSiteCode = @LabSiteCode
					End	

				IF @Comments = ''
					Begin
						Select @Comments = Comments from dbo.SitePref_LabMailToPatient where LabSiteCode = @LabSiteCode
					End
				--Else
				--	Begin 
				--		Select @Comments = Comments + '  ' + @Comments from dbo.SitePref_LabMailToPatient where LabSiteCode = @LabSiteCode
				--	End

				UPDATE dbo.SitePref_LabMailToPatient SET LabMailToPatient = @LMTP, LMTP_AllClinics = @LMTP_All,
					OrderCapacity = @CapacityNbr, Comments = @Comments, ModifiedBy = @ModifiedBy,
					DateLastModified = GETDATE(), StartDate = @StartDate, StopDate = @StopDate, AnticipatedEndDate = @AnticipatedEndDate,
					StatusReason = @StatusReason
				WHERE LabSiteCode = @LabSiteCode
				
			COMMIT TRANSACTION
		END
		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[IUSitePref_Lab]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 04/04/2019
-- Description:	Used to insert or update records in the SitePref_Lab table
-- =============================================
CREATE PROCEDURE [dbo].[IUSitePref_Lab]
	-- Add the parameters for the stored procedure here
	@LabSiteCode VarChar(6),
	@MaxPrism Decimal(4,2) = Null,
	@MaxDecentrationPlus Decimal(4,2) = Null,
	@MaxDecentrationMinus Decimal(4,2) = Null,
	@PatientDirectedOrders Bit = Null,
	@Comment VarChar(250) = Null,
	@ModifiedBy VarChar(200),
	@Success Bit OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Declare @RecCnt int = 0,
		@RecComment VarChar(250),
		@ErrorLogID Int = 0

	SELECT @RecCnt = Count(*) from dbo.SitePref_Lab where LabSiteCode = @LabSiteCode
	Select @RecComment = Comment from dbo.SitePref_Lab where LabSiteCode = @LabSiteCode

	BEGIN TRY
		If @RecCnt = 1
			Begin
				If @RecComment is Not Null
					Begin
						Set @Comment = @RecComment + '   ' + @Comment
					End
					BEGIN TRANSACTION
						Update dbo.SitePref_Lab set MaxPrism = @MaxPrism, MaxDecentrationMinus = @MaxDecentrationMinus,
							MaxDecentrationPlus = @MaxDecentrationPlus, PatientDirectedOrders = @PatientDirectedOrders,
							Comment = @RecComment, ModifiedBy = @ModifiedBy, DateLastModified = GetDate()
					COMMIT TRANSACTION
			End
		IF @RecCnt = 0
			Begin
				BEGIN TRANSACTION
					Insert into dbo.SitePref_Lab 
					Values (@LabSiteCode, @MaxPrism, @MaxDecentrationPlus,@MaxDecentrationMinus, @PatientDirectedOrders,
						@Comment, @ModifiedBy, GetDate())
				COMMIT TRANSACTION
			End

			SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSitePref_Rx]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 31 May 2017
-- Description:	Insert/Update Rx Prefrences for a Clinic
-- =============================================
CREATE PROCEDURE [dbo].[InsertSitePref_Rx] 
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@RxType VARCHAR(5) = Null,
	@Provider INT = Null,
	@PDDistance DECIMAL(6,2) = Null,
	@PDNear DECIMAL(6,2) = Null,
	@ModifiedBy VARCHAR(200),
	@Success BIT OUTPUT
AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_Rx 
	WHERE SiteCode = @SiteCode 
	
	BEGIN TRY
		IF @RecCnt = 0
		BEGIN	
			BEGIN TRANSACTION
				INSERT INTO dbo.SItePref_Rx(SiteCode, RxType, Provider, PDDistance, PDNear, ModifiedBy, DateLastModified)
				VALUES (@SiteCode, @RxType,@Provider,  @PDDistance, @PDNear, @ModifiedBy, GETDATE())
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			BEGIN TRANSACTION
				UPDATE dbo.sitePref_Rx SET Provider = @Provider, RxType = @RxType,
					PDDistance = @PDDistance, PDNear = @PDNear, ModifiedBy = @ModifiedBy,
					DateLastModified = GETDATE()
				WHERE SiteCode = @SiteCode
			COMMIT TRANSACTION
		END
		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSitePref_Orders]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 31 May 2017
-- Description:	Insert/Update Site Preference - Orders
-- =============================================
CREATE PROCEDURE [dbo].[InsertSitePref_Orders]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@Priority CHAR(1),
	@DispenseMethod INT,
	@Lab VARCHAR(6),
	@Pair INT = 1,
	@ModifiedBy VARCHAR(200),
	@IPriority CHAR(1) = Null,
	@IFrame VARCHAR(15) = NULL,
	@Frame VARCHAR(15) = Null,
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @RecCnt INT = 0,
			@ErrorLogID INT = 0
 
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_Orders WHERE SiteCode = @SiteCode AND Priority = @Priority
	
	IF @Priority is NULL	
		BEGIN
			SET  @Priority = @IPriority 
		END
	IF @Frame IS NULL
	BEGIN
		SET @Frame = @IFrame
	END
	Begin TRY
		IF @RecCnt = 0
		BEGIN
			BEGIN TRANSACTION
				INSERT INTO dbo.SitePref_Orders (SiteCode, Priority, DispenseMethod, ProdLab, Pair, ModifiedBy,
					DateLastModified,  IPriority, IFrame, Frame)
				VALUES (@SiteCode, @Priority, @DispenseMethod, @Lab, @Pair, @ModifiedBy, GETDATE(), @IPriority, @IFrame, @Frame)
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			BEGIN TRANSACTION
				UPDATE dbo.SitePref_Orders SET [Priority] = @Priority, DispenseMethod = @DispenseMethod, ProdLab = @Lab,
					Pair = @Pair,  ModifiedBy = @ModifiedBy, DateLastModified = GETDATE(), IFrame = @IFrame,
					IPriority = @IPriority, Frame = @Frame
				WHERE SiteCode = @SiteCode AND Priority = @Priority
			COMMIT TRANSACTION
		END
	SET @Success = 1
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSitePref_Justifications]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 August 2017
-- Description:	Used to insert/update site preference justifications
-- =============================================
CREATE PROCEDURE [dbo].[InsertSitePref_Justifications]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@Reason VARCHAR(50),
	@Justify VARCHAR(500) = NULL,
	@Success INT OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Rsn VARCHAR(50) = NULL,
		@RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) --@SC = SiteCode, @FC = @FrameCode 
	FROM dbo.SitePref_Justifications
	WHERE SiteCode = @SiteCode AND Reason = @Reason
	
	BEGIN TRY
		IF @RecCnt = 0
		BEGIN
			BEGIN TRANSACTION
				INSERT INTO dbo.SitePref_Justifications
				VALUES (@SiteCode, @Reason, @Justify)
			COMMIT TRANSACTION
		END
	ELSE -- Update existing record	
		BEGIN
			BEGIN TRANSACTION
				UPDATE dbo.SitePref_Justifications
				SET Justification = @Justify
				WHERE SiteCode = @SiteCode AND Reason = @Rsn
			COMMIT TRANSACTION
		END
		SET @Success = 1
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSitePref_General]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 23 June 2017
-- Description:	Insert Site Code Preferences = General
-- =============================================
CREATE PROCEDURE [dbo].[InsertSitePref_General]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@AlphaSort BIT,
	@Comments VARCHAR(500) = Null,
	@CommentType VARCHAR(25) = NULL,
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_General
	WHERE SiteCode = @SiteCode
	
	BEGIN TRY
		IF @RecCnt = 0
		BEGIN
			BEGIN TRANSACTION
				INSERT INTO dbo.SitePref_General(SiteCode, AlphaSort, Comments, CommentType)
				VALUES (@SiteCode, @AlphaSort, @Comments, @CommentType)
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			BEGIN TRANSACTION
				UPDATE dbo.SitePref_General SET AlphaSort = @AlphaSort,
					Comments = @Comments, CommentType = @CommentType
				WHERE SiteCode = @SiteCode
			COMMIT TRANSACTION
		END
		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSitePref_Frames]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 June 2017
-- Description:	Insert/Update Site Preferences for Frames
-- =============================================
CREATE PROCEDURE [dbo].[InsertSitePref_Frames]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@FrameCode VARCHAR(15),
	@Color VARCHAR(40),
	@Eye VARCHAR(40),
	@Bridge VARCHAR(40),
	@Temple VARCHAR(40),
	@Lens_Type VARCHAR(40),
	@Tint VARCHAR(40),
	@Material VARCHAR(40),
	@Coatings VARCHAR(40),
	@ODSegHeight VARCHAR(2),
	@OSSegHeight VARCHAR(2),
	@ModifiedBy VARCHAR(200),
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SC VARCHAR(6) = NULL,
		@FC VARCHAR(15) = NULL,
		@RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) --@SC = SiteCode, @FC = @FrameCode 
	FROM dbo.SitePref_Frames 
	WHERE SiteCode = @SiteCode AND Frame = @FrameCode
	
	BEGIN TRY
		IF @RecCnt = 0
		BEGIN
			BEGIN TRANSACTION
				INSERT INTO dbo.SitePref_Frames (SiteCode, Frame, Color, Eye, Bridge, Temple,
					Lens_Type, Tint, Material, Coatings, ODSegHeight, OSSegHeight, ModifiedBy, DateLastModified)
				VALUES (@SiteCode, @FrameCode, @Color, @Eye, @Bridge, @Temple, @Lens_Type,
					@Tint, @Material, @Coatings, @ODSegHeight, @OSSegHeight, @ModifiedBy, GETDATE())
			COMMIT TRANSACTION
		END
		ELSE
		BEGIN
			BEGIN TRANSACTION
				UPDATE dbo.SitePref_Frames SET Color = @Color, Eye = @Eye, Bridge = @Bridge,
					Temple = @Temple, Lens_Type = @Lens_Type, Tint = @Tint, Material = @Material,
					Coatings = @Coatings, ODSegHeight = @ODSegHeight, OSSegHEight = @OSSegHeight,
					ModifiedBy = @ModifiedBy, DateLastModified = GETDATE()
				WHERE SiteCode = @SiteCode AND Frame = @FrameCode
			COMMIT TRANSACTION		
		END
		SET @Success = 1
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSiteAddress]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertSiteAddress] 
@Address1	varchar(200),
@Address2	varchar(200),
@Address3	varchar(200),
@City		varchar(200),
@State		varchar(2),
@Country	varchar(2),
@ZipCode	varchar(10),
@IsConus	bit,
@AddressType	varchar(4),
@IsActive	bit,
@ModifiedBy		VARCHAR(200),
@SiteCode	varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT 
		
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT dbo.SiteAddress
			        ( SiteCode ,
			          Address1 ,
			          Address2 ,
			          Address3 ,
			          City ,
			          [State] ,
			          Country ,
			          ZipCode ,
			          IsConus ,
			          AddressType ,
			          IsActive ,
			          ModifiedBy ,
			          DateLastModified ,
			          LegacyID
			        )
			VALUES  ( @SiteCode , -- SiteCode - varchar(6)
			          @Address1 , -- Address1 - varchar(200)
			          @Address2 , -- Address2 - varchar(200)
			          @Address3 , -- Address3 - varchar(200)
			          @City , -- City - varchar(200)
			          @State , -- State - varchar(2)
			          @Country , -- Country - varchar(2)
			          @ZipCode , -- ZipCode - varchar(10)
			          1 , -- IsConus - bit
			          @AddressType , -- AddressType - varchar(4)
			          @IsActive , -- IsActive - bit
			          @ModifiedBy , -- ModifiedBy - varchar(200)
			          GETDATE() , -- DateLastModified - datetime
			          0  -- LegacyID - int
			        )
		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
		SELECT SiteCode,
		Address1,
		Address2,
		Address3,
		City,
		[State],
		Country,
		ZipCode,
		IsConus,
		AddressType,
		IsActive,
		ModifiedBy,
		DateLastModified
		FROM dbo.SiteAddress
		WHERE SiteCode = @SiteCode AND AddressType = @AddressType
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertSite]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertSite] 
@SiteCode	varchar(6),
@SiteName	varchar(50),
@SiteType	varchar(50),
@SiteDescription	varchar(100),
@BOS		varchar(1),
@IsMultivision	bit,
@EMailAddress	varchar(512),
@DSNPhoneNumber	varchar(20),
@RegPhoneNumber	varchar(20),
@IsAPOCompatible	bit,
@MaxEyeSize		int,
@MaxFramesPerMonth int,
@MaxPower	numeric(18,0),
@HasLMS		bit,
@Region		int,
@MultiPrimary	varchar(6),
@MultiSecondary	varchar(6),
@SinglePrimary	varchar(6),
@SingleSecondary	varchar(6),
@ShipToPatientLab bit,
@IsActive	bit,
@IsReimbursable  bit = 0,
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT 
		
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT dbo.SiteCode
			        ( SiteCode ,
			          SiteName ,
			          SiteType ,
			          SiteDescription ,
			          BOS ,
			          IsMultivision ,
			          EMailAddress ,
			          DSNPhoneNumber ,
			          RegPhoneNumber ,
			          IsAPOCompatible ,
			          MaxEyeSize ,
			          MaxFramesPerMonth ,
			          MaxPower ,
			          HasLMS ,
			          Region ,
			          MultiPrimary ,
			          MultiSecondary ,
			          SinglePrimary ,
			          SingleSecondary ,
			          ShipToPatientLab,
			          IsActive ,
			          IsReimbursable,
			          ModifiedBy ,
			          DateLastModified ,
			          LegacyID
			        )
			VALUES  ( @SiteCode , -- SiteCode - varchar(6)
			          @SiteName , -- SiteName - varchar(50)
			          @SiteType , -- SiteType - varchar(50)
			          @SiteDescription , -- SiteDescription - varchar(100)
			          @BOS , -- BOS - varchar(1)
			          @IsMultivision , -- IsMultivision - bit
			          @EmailAddress , -- EMailAddress - varchar(512)
			          @DSNPhoneNumber , -- DSNPhoneNumber - varchar(20)
			          @RegPhoneNumber , -- RegPhoneNumber - varchar(20)
			          0 , -- IsAPOCompatible - bit
			          @MaxEyeSize , -- MaxEyeSize - int
			          @MaxFramesPerMonth , -- MaxFramesPerMonth - int
			          @MaxPower , -- MaxPower - numeric
			          @HasLMS , -- HasLMS - bit
			          0 , -- Region - int
			          @MultiPrimary , -- MultiPrimary - varchar(6)
			          @MultiSecondary , -- MultiSecondary - varchar(6)
			          @SinglePrimary , -- SinglePrimary - varchar(6)
			          @SingleSecondary , -- SingleSecondary - varchar(6)
			          @ShipToPatientLab, -- ShipToPatientLab - bit
			          @IsActive , -- IsActive - bit
			          @IsReimbursable, -- bit
			          @ModifiedBy , -- ModifiedBy - varchar(200)
			          GETDATE() , -- DateLastModified - datetime
			          
			          0  -- LegacyID - int
			        )
		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
		SELECT SiteCode,
		SiteName,
		SiteType,
		SiteDescription,
		BOS,
		IsMultivision,
		EMailAddress,
		DSNPhoneNumber,
		RegPhoneNumber,
		IsAPOCompatible,
		MaxEyeSize,
		MaxFramesPerMonth,
		MaxPower,
		HasLMS,
		Region,
		MultiPrimary,
		MultiSecondary,
		SinglePrimary,
		SingleSecondary,
		ShipToPatientLab,
		IsActive,
		IsReimbursable,
		ModifiedBy,
		DateLastModified
		FROM dbo.SiteCode
		WHERE SiteCode = @SiteCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertAuthorizationByUserName]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
	Modified:  11/21/2016 - baf - To remove SiteCode information
*/

CREATE PROCEDURE [dbo].[InsertAuthorizationByUserName]
(
	@UserName nvarchar(256)
)
AS
	SET NOCOUNT OFF;

declare @userid as uniqueidentifier
--declare @sitecode as varchar(6)
--declare @sitecodes as table( rowId tinyint, profileXml xml)
--set @UserName='labtech'
set @UserId = CONVERT(uniqueidentifier,(select UserId from aspnet_Users where UserName=@username))

--insert into @sitecodes(rowId, profileXml) values(1, (select PropertyValuesString from aspnet_Profile where UserId=@UserId))

--set @sitecode = convert( varchar(6),(select profileXml.value('(/Personal/SiteCode)[1]','varchar(6)') as SiteCode from @sitecodes))

IF NOT EXISTS -- 11/21/2016 - baf - added NOT 
(
	select [dbo].[Authorization].UserName
	from [dbo].[Authorization]
	where @UserName = [dbo].[Authorization].UserName
)
--BEGIN
--	UPDATE [dbo].[Authorization] set SiteCode = @sitecode
--	where @UserName = [dbo].[Authorization].UserName 
--END

--ELSE
BEGIN
	--INSERT INTO [dbo].[Authorization] 
	--   ([SiteCode], [UserName]) values
	--   (@sitecode, @UserName)
		INSERT INTO [dbo].[Authorization] 
	   ( [UserName]) values
	   (@UserName)
END
GO
/****** Object:  StoredProcedure [dbo].[GetSyncedUser]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 11 Nov 2014
-- Description:	Returns the individual id of the individual that is synced to a user via the username
-- Modified:  12 Apr 2016 - baf - rewrite to validate user has CMS capability
-- =============================================
CREATE PROCEDURE [dbo].[GetSyncedUser]
	-- Add the parameters for the stored procedure here
	--@UserName	VARCHAR(45)
	@UserID INT,
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;
	DECLARE  @aspnetUser UNIQUEIDENTIFIER = null

	SET @Success = 0
	
	SELECT @aspnetUser = aspnet_UserID FROM dbo.Individual WHERE ID = @UserID
	
	IF @aspnetUser IS NOT NULL
	BEGIN
		SET @Success = 1
	END
	

	--SELECT i.ID
	--FROM aspnet_Users u
	--INNER JOIN Individual i on u.UserId = i.aspnet_UserID
	--WHERE u.LoweredUserName = @UserName
END
GO
/****** Object:  StoredProcedure [dbo].[InsertLensStock]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 13 Feb 2017
-- Description:	Allows for insertion of Lens Stock for a particular Lab,
--		Stock is by Material, Cylinder, and Max Plus and Max Minus Sphere sizes
-- Modified: 04/04/19 - baf - Added SV or MV Capability
-- =============================================
CREATE PROCEDURE [dbo].[InsertLensStock]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6),
	@Material VarChar(5),
	@Cylinder Decimal (4,2),
	@MaxPlus Decimal (4,2),
	@MaxMinus Decimal(4,2),
	@IsStocked Bit = 0,
	@IsActive BIT = 1,
	@ModifiedBy VarChar(200),
	@Success	INT OUTPUT,
	@Capability Char(2)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT = 0, @ID INT

	BEGIN TRY
		Begin Transaction
		Insert into dbo.LensStock (SiteCode, Material, Cylinder, MaxPlus, MaxMinus, IsStocked, IsActive, DateLastModified,
			ModifiedBy, Capability)
		values (@SiteCode, @Material, @Cylinder, @MaxPlus, @MaxMinus, @IsStocked, @IsActive, GETDATE(), @ModifiedBy, @Capability)
		
		Set @Success = 1
		Commit Transaction
	END TRY
	BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			SET @Success = 0
			RETURN
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertLabelsOnDemand]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 28 August 2017
-- Description:	Inserts records into LabelsOnDemand Table
-- =============================================
CREATE PROCEDURE [dbo].[InsertLabelsOnDemand]
	-- Add the parameters for the stored procedure here
	@OrderNumber VARCHAR(16),
	@SiteCode VARCHAR(6),
	@AddressType VARCHAR(5),
	@ModifiedBy VARCHAR(200),
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID INT
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT 
	
    -- Insert statements for procedure here
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO dbo.LabelsOnDemand
			VALUES (@OrderNumber, @SiteCode, @AddressType, CAST(GETDATE() AS SMALLDATETIME), 
				@ModifiedBy)
		COMMIT TRANSACTION
		SET @Success = 1
	END TRY
	BEGIN CATCH
	  IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
		SET @Success = 0
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameEligibilityUnion]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameEligibilityUnion] 
@EligibilityCode	varchar(8),
@FrameCode			varchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT EligibilityCode FROM dbo.EligibilityFrameUnion
	WHERE EligibilityCode = @EligibilityCode AND FrameCode = @FrameCode)
	BEGIN 
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.EligibilityFrameUnion
				(EligibilityCode, FrameCode)
				VALUES(@EligibilityCode, @FrameCode)

			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	SELECT EligibilityCode, FrameCode FROM dbo.EligibilityFrameUnion
	WHERE EligibilityCode = @EligibilityCode AND FrameCode = @FrameCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameEligibilityFrameUnion]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameEligibilityFrameUnion] 
@EligibilityCode	varchar(8),
@FrameCode			varchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT EligibilityFrame FROM dbo.FrameEligibilityFrameUnion
	WHERE EligibilityFrame = @EligibilityCode AND FrameCode = @FrameCode)
	BEGIN 
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.FrameEligibilityFrameUnion
				(EligibilityFrame, FrameCode)
				VALUES(@EligibilityCode, @FrameCode)

			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	SELECT EligibilityFrame, FrameCode FROM dbo.FrameEligibilityFrameUnion
	WHERE EligibilityFrame = @EligibilityCode AND FrameCode = @FrameCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrameEligibility]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertFrameEligibility]
	-- Add the parameters for the stored procedure here
	@EligibilityFrame VARCHAR(8),
	@IsActive BIT,
	@ModifiedBy VARCHAR(200),
	@DateLastModified DATETIME 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT EligibilityFrame FROM dbo.FrameEligibility
	WHERE EligibilityFrame = @EligibilityFrame) 
	BEGIN 
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.FrameEligibility
				(EligibilityFrame, IsActive, ModifiedBy, DateLastModified)
				VALUES(@EligibilityFrame, @IsActive, @ModifiedBy, GETDATE())

			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	SELECT * FROM dbo.FrameEligibility
	WHERE EligibilityFrame = @EligibilityFrame
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertEligibilityFrameAndUnion]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertEligibilityFrameAndUnion] 
@EligibilityCode	varchar(7),
@FrameCode			VARCHAR(15),
@FrameDescription	VARCHAR(100),
@FrameNotes			VARCHAR(100),
@IsInsert			BIT,
@MaxPair			INT,
@ImageURL			VARCHAR(250) = '',
@IsActive			BIT,
@ModifiedBy			varchar(200),
@IsFOC				Bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	DECLARE @TempCode varchar(8)
	
	IF NOT EXISTS
	(SELECT EligibilityFrame FROM dbo.FrameEligibility
	WHERE EligibilityFrame = @EligibilityCode)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.FrameEligibility
				(EligibilityFrame, IsActive, ModifiedBy, DateLastModified)
				VALUES(@EligibilityCode, 1, @ModifiedBy, GETDATE())

			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	IF NOT EXISTS
	(SELECT FrameCode FROM dbo.Frame
	WHERE FrameCode = @FrameCode)
	BEGIN
		INSERT INTO dbo.Frame
		        ( FrameCode ,
		          FrameDescription ,
		          FrameNotes ,
		          IsInsert ,
		          MaxPair ,
		          ImageURL ,
		          IsActive ,
		          ModifiedBy ,
		          DateLastModified,
		          IsFOC
		        )
		VALUES  ( @FrameCode , -- FrameCode - varchar(15)
		          @FrameDescription , -- FrameDescription - varchar(100)
		          @FrameNotes , -- FrameNotes - varchar(100)
		          @IsInsert , -- IsInsert - bit
		          @MaxPair , -- MaxPair - int
		          '' , -- ImageURL - varchar(250)
		          1 , -- IsActive - bit
		          @ModifiedBy , -- ModifiedBy - varchar(200)
		          GETDATE(),   -- DateLastModified - datetime
		          @IsFOC
		        )
	END
	
	IF NOT EXISTS
	(SELECT EligibilityFrame FROM dbo.FrameEligibilityFrameUnion
	WHERE EligibilityFrame = @EligibilityCode AND FrameCode = @FrameCode)
	BEGIN
		INSERT INTO dbo.FrameEligibilityFrameUnion
		        ( EligibilityFrame, FrameCode )
		VALUES  ( @EligibilityCode, -- EligibilityCode - varchar(8)
		          @FrameCode  -- FrameCode - varchar(15)
		          )	
	END
	
	SELECT EligibilityFrame, IsActive, ModifiedBy, DateLastModified FROM dbo.FrameEligibility
	WHERE EligibilityFrame = @EligibilityCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertEligibility]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertEligibility] 
@EligibilityCode	varchar(8),
@ModifiedBy			varchar(200),
@FrameCode		VARCHAR(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	DECLARE @TempCode varchar(8)
	
	IF NOT EXISTS
	(SELECT EligibilityCode FROM dbo.Eligibility
	WHERE EligibilityCode = @EligibilityCode)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.Eligibility
				(EligibilityCode, IsActive, ModifiedBy, DateLastModified)
				VALUES(@EligibilityCode, 1, @ModifiedBy, GETDATE())

			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	IF NOT EXISTS
	(SELECT EligibilityCode FROM dbo.EligibilityFrameUnion
	WHERE EligibilityCode = @EligibilityCode AND FrameCode = @FrameCode)
	BEGIN
		INSERT INTO dbo.EligibilityFrameUnion
		        ( EligibilityCode, FrameCode )
		VALUES  ( @EligibilityCode, -- EligibilityCode - varchar(8)
		          @FrameCode  -- FrameCode - varchar(15)
		          )	
	END
	
	SELECT EligibilityCode, IsActive, ModifiedBy, DateLastModified FROM dbo.Eligibility
	WHERE EligibilityCode = @EligibilityCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertFrame]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  8/14/15 - baf added ImageURL to the 
--	parameter list.
-- ==========================================
CREATE PROCEDURE [dbo].[InsertFrame] 
@FrameCode		VARCHAR(15),
@FrameDescription	VARCHAR(100),
@FrameNotes			VARCHAR(100),
@IsInsert			BIT,
@MaxPair			INT,
@ModifiedBy			VARCHAR(200),
@IsFOC				BIT,
@ImageURL			VARCHAR(250) = '',
@IsActive			Bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	IF NOT EXISTS
	(SELECT FrameCode FROM dbo.Frame
	WHERE FrameCode = @FrameCode)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO dbo.Frame
				        ( FrameCode ,
				          FrameDescription ,
				          FrameNotes ,
				          IsInsert ,
				          MaxPair ,
				          ImageURL ,
				          IsActive ,
				          ModifiedBy ,
				          DateLastModified,
				          IsFOC
				        )
				VALUES  ( @FrameCode, -- FrameCode - varchar(15)
				          @FrameDescription, -- FrameDescription - varchar(50)
				          @FrameNotes, -- FrameNotes - varchar(100)
				          @IsInsert, -- IsInsert - bit
				          @MaxPair, -- MaxPair - int
				          @ImageURL , -- ImageURL - varchar(250)
				          @IsActive, -- IsActive - bit
				          @ModifiedBy, -- ModifiedBy - varchar(200)
				          GETDATE(),  -- DateLastModified - datetime
				          @IsFOC
				        )		
			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
		END CATCH
	END
	
	SELECT FrameCode, FrameDescription, FrameNotes, IsInsert, MaxPair, ImageURL, IsActive, ModifiedBy, DateLastModified, IsFOC FROM dbo.Frame
	WHERE FrameCode = @FrameCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitesBySiteTypeAndRegion]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSitesBySiteTypeAndRegion] 
	@SiteType varchar(50),
	@Region int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.SiteName
		,a.SiteType
		,a.SiteDescription
		,a.BOS
		,a.IsMultivision
		,b.ID
		,b.Address1
		,b.Address2
		,b.Address3
		,b.City
		,b.[State]
		,b.Country
		,b.ZipCode
		,b.IsConus
		,a.EMailAddress
		,a.DSNPhoneNumber
		,a.RegPhoneNumber
		,a.IsAPOCompatible
		,a.MaxEyeSize
		,a.MaxFramesPerMonth
		,a.MaxPower
		,a.HasLMS
		,a.Region
		,a.MultiPrimary
		,a.MultiSecondary
		,a.SinglePrimary
		,a.SingleSecondary
		,a.ShipToPatientLab
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
	from dbo.SiteCode a
	inner join dbo.SiteAddress b on a.SiteCode = b.SiteCode
	where a.SiteType = @SiteType AND a.Region = @Region 
	AND b.AddressType = 'SITE'
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitesBySiteType]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  12/15/2017 - baf - To pull only active sites
-- =============================================
CREATE PROCEDURE [dbo].[GetSitesBySiteType] 
	@SiteType varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.SiteName
		,a.SiteType
		,a.SiteDescription
		,a.BOS
		,a.IsMultivision
		,b.ID
		,b.Address1
		,b.Address2
		,b.Address3
		,b.City
		,b.[State]
		,b.Country
		,b.ZipCode
		,b.IsConus
		,a.EMailAddress
		,a.DSNPhoneNumber
		,a.RegPhoneNumber
		,a.IsAPOCompatible
		,a.MaxEyeSize
		,a.MaxFramesPerMonth
		,a.MaxPower
		,a.HasLMS
		,a.Region
		,a.MultiPrimary
		,a.MultiSecondary
		,a.SinglePrimary
		,a.SingleSecondary
		,a.ShipToPatientLab
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
	from dbo.SiteCode a
	inner join dbo.SiteAddress b on a.SiteCode = b.SiteCode
	where SiteType = @SiteType and a.IsActive = 1
	AND b.AddressType = 'SITE'
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePrefLabs]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 26 July 2018
-- Description:	Used to retrieve Lab Site Preferences
-- =============================================
CREATE PROCEDURE [dbo].[GetSitePrefLabs]
	-- Add the parameters for the stored procedure here
	@LabSite VARCHAR(6),
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID INT = 0,
		@RecCnt  INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_Lab
	WHERE LabSiteCode = @LabSite
		
    BEGIN TRY
		IF @RecCnt = 1
		Begin
			SELECT * FROM dbo.SitePref_Lab WHERE LabSiteCode = @LabSite
			SET @Success = 1
		END
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN
				-- Select Failed
				SET @Success = 0
			END		
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT		
	END Catch
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePref_Rx]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 31 May 2017
-- Description:	Get Site Preferences for Rx
-- =============================================
CREATE PROCEDURE [dbo].[GetSitePref_Rx]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Select * from dbo.SitePref_Rx where SiteCode = @SiteCode

END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePref_Orders]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 31 May 2017
-- Description:	Get Site Preferences for Orders
-- =============================================
CREATE PROCEDURE [dbo].[GetSitePref_Orders]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT (spo.SiteCode + ' - ' + SiteName) as SiteInfo, spo.Priority AS PriorityCode, [Text] AS [Priority],
		CASE WHEN DispenseMethod = 1 THEN 'Clinic Distribution'
			WHEN DispenseMethod = 2 THEN 'Clinic Ship To Patient'
			when DispenseMethod = 3 THEN 'Lab Ship To Patient'
		ELSE '' END DispenseMethod,
		CASE WHEN LEFT(ProdLab,1) = 'S' THEN 'SV - ' + ProdLab
			WHEN LEFT(ProdLab,1) = 'M' THEN 'MV - ' + ProdLab 
		END ProdLab, Pair, IPriority, IFrame, Frame,
		Fr.FrameDescription
	FROM dbo.SitePref_Orders spo
		INNER JOIN dbo.LookupTable lu ON spo.Priority = lu.Value
		INNER JOIN dbo.SiteCode sc ON spo.SiteCode = sc.SiteCode
		INNER JOIN dbo.Frame fr ON spo.Frame = fr.framecode
	WHERE spo.SiteCode = @SiteCode AND lu.Code = 'OrderPriorityType'
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePref_LabMailToPatient]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 26 July 2018
-- Description:	Used to retrieve Lab MailToPatient Preferences
-- =============================================
CREATE PROCEDURE [dbo].[GetSitePref_LabMailToPatient]
	-- Add the parameters for the stored procedure here
	@LabSite VARCHAR(6),
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID INT = 0,
		@RecCnt  INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_LabMailToPatient
	WHERE LabSiteCode = @LabSite
		
    BEGIN TRY
		IF @RecCnt = 1
		Begin
			SELECT * FROM dbo.SitePref_LabMailToPatient WHERE LabSiteCode = @LabSite
			SET @Success = 1
		END
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN
				-- Select Failed
				SET @Success = 0
			END		
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT		
	END Catch
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePref_Justifications]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 August 2017
-- Description:	Retrieves Justification records for a site
-- =============================================
CREATE PROCEDURE [dbo].[GetSitePref_Justifications]
	
	@SiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.SitePref_Justifications 
	WHERE SiteCode = @SiteCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePref_General]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 23 June 2017
-- Description:	Retrieve General Defaults for a site code
-- =============================================
CREATE PROCEDURE [dbo].[GetSitePref_General]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.SitePref_General 
	WHERE SiteCode = @SiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetSitePref_Frames]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSitePref_Frames]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT (spf.SiteCode + ' - ' + SiteName) AS SiteInfo, Frame, Color, Eye,
		Bridge, Temple, Lens_Type, Tint, Material, Replace(Coatings,'/',',') as Coatings, ODSegHeight, OSSegHeight
	FROM dbo.SitePref_Frames spf
		INNER JOIN dbo.SiteCode sc ON spf.SiteCode = sc.SiteCode
	WHERE spf.SiteCode = @SiteCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetSiteBySiteID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSiteBySiteID] 
	-- Add the parameters for the stored procedure here
	@SiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.SiteName
		,a.SiteType
		,a.SiteDescription
		,a.BOS
		,a.IsMultivision
		,b.ID
		,b.Address1
		,b.Address2
		,b.Address3
		,b.City
		,b.[State]
		,b.Country
		,b.ZipCode
		,b.IsConus
		,b.AddressType
		,a.EMailAddress
		,a.DSNPhoneNumber
		,a.RegPhoneNumber
		,a.IsAPOCompatible
		,a.MaxEyeSize
		,a.MaxFramesPerMonth
		,a.MaxPower
		,a.HasLMS
		,a.Region
		,a.MultiPrimary
		,a.MultiSecondary
		,a.SinglePrimary
		,a.SingleSecondary
		,a.ShipToPatientLab
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
	from dbo.SiteCode a
	LEFT OUTER join dbo.SiteAddress b on a.SiteCode = b.Sitecode
	where a.SiteCode = @SiteCode
--	AND b.AddressType = 'SITE'
END
GO
/****** Object:  StoredProcedure [dbo].[GetSiteAddressBySiteID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSiteAddressBySiteID] 
	-- Add the parameters for the stored procedure here
	@SiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.ID
		,a.Address1
		,a.Address2
		,a.Address3
		,a.City
		,a.[State]
		,a.Country
		,a.ZipCode
		,a.AddressType
		,a.IsConus
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
	from dbo.SiteAddress a
	where a.SiteCode = @SiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetAspnet_UserName]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 2/9/2015
-- Description:	This stored procedure gets the UserName
--		from aspnet_Profile when ID is supplied
-- =============================================
CREATE PROCEDURE [dbo].[GetAspnet_UserName]
	-- Add the parameters for the stored procedure here
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select u.UserName
	from aspnet_Profile p
	inner join aspnet_Users u on p.UserID = u.UserID
	where Convert(XML,p.PropertyValuesString).value ('(//Personal/IndividualId)[1]','int') = @id;
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllSites]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllSites] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.SiteCode
		,a.SiteName
		,a.SiteType
		,a.SiteDescription
		,a.BOS
		,a.IsMultivision
		,b.ID
		,b.Address1
		,b.Address2
		,b.Address3
		,b.City
		,b.[State]
		,b.Country
		,b.ZipCode
		,b.IsConus
		,a.EMailAddress
		,a.DSNPhoneNumber
		,a.RegPhoneNumber
		,a.IsAPOCompatible
		,a.MaxEyeSize
		,a.MaxFramesPerMonth
		,a.MaxPower
		,a.IsActive
		,a.ModifiedBy
		,a.DateLastModified
		,a.HasLMS
		,a.Region
		,a.MultiPrimary
		,a.MultiSecondary
		,a.SinglePrimary
		,a.SingleSecondary
		,a.ShipToPatientLab
	from dbo.SiteCode a
	inner join dbo.SiteAddress b on a.SiteCode = b.SiteCode
	where b.AddressType = 'SITE'
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetEmailMsg]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 06 March 2019
-- Description:	Used to retrieve a record from EmailMsg
--		Table to be used for a patient's order email
-- Modified:
-- =============================================
CREATE PROCEDURE [dbo].[GetEmailMsg]
	-- Add the parameters for the stored procedure here
	@EmailType VarChar(50),
	@Msg1 VarChar(500) OUTPUT,
	@Msg2 VarChar(500) OUTPUT,
	@Msg3 VarChar(500) OUTPUT,
	@Msg4 VarChar(500) OUTPUT,
	--@EmailMsg VarChar(Max) = '' OUTPUT,
	@Success INT Output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID INT = 0,
		@RecCnt  INT = 0
		
    BEGIN TRY	
		-- Retrieve Record by Order Number
		If @EmailType = 'Order-Mail'
			Begin	
				Select @Msg1 = MsgPart1, @Msg2 = MsgPart2, @Msg3 = MsgPart3, @Msg4 = MsgPart4
				from dbo.EmailMsg where EmailType = @EmailType
			END
		
		-- Retrieve all records by LabSiteCode
		ELSE If @EmailType = 'Order-Clinic'
			Begin
				Select @Msg1 = MsgPart1, @Msg2 = MsgPart2, @Msg3 = MsgPart3, @Msg4 = MsgPart4
				from dbo.EmailMsg where EmailType = @EmailType
			END
		SET @Success = 1
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN
				-- Select Failed
				SET @Success = 0
			END		
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT		
	END Catch
END
GO
/****** Object:  StoredProcedure [dbo].[GetEligibilityByFrameCode]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEligibilityByFrameCode] 
@FrameCode	VARCHAR(15) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT EligibilityFrame FROM dbo.FrameEligibilityFrameUnion
	WHERE FrameCode = @FrameCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByRecipientTypeID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByRecipientTypeID] 
@RecipientTypeID	varchar(5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentRecipientTypeID = @RecipientTypeID
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByRecipientGroupID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByRecipientGroupID] 
@RecipientGroupID	int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentRecipientGroupID = @RecipientGroupID
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByExpireDate]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByExpireDate] 
@ExpireDate	datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentExpireDate <= @ExpireDate
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByDisplayDate]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByDisplayDate] 
@DisplayDate	datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as ArthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentDisplayDate >= @DisplayDate
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByContentTypeID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByContentTypeID] 
@ContentTypeID	varchar(5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @currDt datetime = DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()));
	
    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentRecipientSiteID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	,a.cmsCreatedDate
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentTypeID = @ContentTypeID
	and @currDt between a.cmsContentDisplayDate and a.cmsContentExpireDate
	ORDER BY a.cmsCreatedDate desc;

END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByContentID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByContentID] 
@ContentID	int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentRecipientSiteID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	,cmsCreatedDate
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentID = @ContentID
	ORDER BY a.cmsCreatedDate desc;
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMSMessageByAuthorID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCMSMessageByAuthorID] 
@AuthorID	int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	WHERE a.cmsContentAuthorID = @AuthorID
END
GO
/****** Object:  StoredProcedure [dbo].[GetCms_MessageCenter_ApplicationAnnouncement_Content]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 14 May 14
-- Description:	Gets all of the messages for the Message Center and Application Announcement types and all
--				sub-types that fall within the specified date range.
-- =============================================
CREATE PROCEDURE [dbo].[GetCms_MessageCenter_ApplicationAnnouncement_Content]
	-- Add the parameters for the stored procedure here
	@contentTypeId varchar(5),
	@siteId varchar(6) = '',
	@siteTypeId varchar(5) = '',
	@individualId int = 0,
	@groupId varchar(5) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @currDt datetime = DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()));
	
    WITH tbl (cmsContentID,cmsContentTitle,cmsContentBody,cmsContentAuthorID,cmsContentRecipientIndividualID,cmsContentRecipientSiteID
		,cmsContentDisplayDate,cmsContentExpireDate, cmsCreatedDate,cmsContentTypeID,cmsContentTypeName,cmsContentDescription
		,cmsRecipientGroupID,cmsRecipientGroupName,cmsRecipientGroupDescription,cmsRecipientTypeID,cmsRecipientTypeName
		,cmsRecipientTypeDescription,AuthorLastName,AuthorFirstname,AuthorMiddleName,RecipientLastName,RecipientFirstName,
		RecipientMiddleName) as 
    (
		select
		a.cmsContentID,a.cmsContentTitle,a.cmsContentBody,a.cmsContentAuthorID,a.cmsContentRecipientIndividualID,a.cmsContentRecipientSiteID
		,a.cmsContentDisplayDate,a.cmsContentExpireDate,a.cmsCreatedDate,b.cmsContentTypeID,b.cmsContentTypeName,b.cmsContentDescription
		,c.cmsRecipientGroupID,c.cmsRecipientGroupName,c.cmsRecipientGroupDescription,d.cmsRecipientTypeID,d.cmsRecipientTypeName
		,d.cmsRecipientTypeDescription,e.LastName as AuthorLastName,e.Firstname as AuthorFirstname ,e.MiddleName as AuthorMiddleName
		,f.LastName as RecipientLastName,f.FirstName as RecipientFirstName,f.MiddleName as RecipientMiddleName
		from dbo.CMS_ContentMessage a
		INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
		LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
		LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
		INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
		LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
		WHERE a.cmsContentTypeID = @ContentTypeID
		and (a.cmsContentRecipientTypeID = 'R001'
			or (a.cmsContentRecipientTypeID = @siteTypeId and a.cmsContentRecipientSiteID = @siteId)
			or (a.cmsContentRecipientTypeID = 'R004' and a.cmsContentRecipientIndividualID = @individualId)
			or (a.cmsContentRecipientTypeID = 'R005' and a.cmsContentRecipientGroupID = @groupId))
		and @currDt between a.cmsContentDisplayDate and a.cmsContentExpireDate
/*
		union

		select
		a.cmsContentID,a.cmsContentTitle,a.cmsContentBody,a.cmsContentAuthorID,a.cmsContentRecipientIndividualID,a.cmsContentRecipientSiteID
		,a.cmsContentDisplayDate,a.cmsContentExpireDate,a.cmsCreatedDate,b.cmsContentTypeID,b.cmsContentTypeName,b.cmsContentDescription
		,c.cmsRecipientGroupID,c.cmsRecipientGroupName,c.cmsRecipientGroupDescription,d.cmsRecipientTypeID,d.cmsRecipientTypeName
		,d.cmsRecipientTypeDescription,e.LastName as AuthorLastName,e.Firstname as AuthorFirstname ,e.MiddleName as AuthorMiddleName
		,f.LastName as RecipientLastName,f.FirstName as RecipientFirstName,f.MiddleName as RecipientMiddleName
		from dbo.CMS_ContentMessage a
		INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
		LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
		LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
		INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
		LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
		where a.cmsContentRecipientTypeID = 'R001'

		union
--in ('R002', 'R003')
		select
		a.cmsContentID,a.cmsContentTitle,a.cmsContentBody,a.cmsContentAuthorID,a.cmsContentRecipientIndividualID,a.cmsContentRecipientSiteID
		,a.cmsContentDisplayDate,a.cmsContentExpireDate,a.cmsCreatedDate,b.cmsContentTypeID,b.cmsContentTypeName,b.cmsContentDescription
		,c.cmsRecipientGroupID,c.cmsRecipientGroupName,c.cmsRecipientGroupDescription,d.cmsRecipientTypeID,d.cmsRecipientTypeName
		,d.cmsRecipientTypeDescription,e.LastName as AuthorLastName,e.Firstname as AuthorFirstname ,e.MiddleName as AuthorMiddleName
		,f.LastName as RecipientLastName,f.FirstName as RecipientFirstName,f.MiddleName as RecipientMiddleName
		from dbo.CMS_ContentMessage a
		INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
		LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
		LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
		INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
		LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
		where (a.cmsContentRecipientTypeID = @siteTypeId
		and a.cmsContentRecipientSiteID = @siteId)

		union

		select
		a.cmsContentID,a.cmsContentTitle,a.cmsContentBody,a.cmsContentAuthorID,a.cmsContentRecipientIndividualID,a.cmsContentRecipientSiteID
		,a.cmsContentDisplayDate,a.cmsContentExpireDate,a.cmsCreatedDate,b.cmsContentTypeID,b.cmsContentTypeName,b.cmsContentDescription
		,c.cmsRecipientGroupID,c.cmsRecipientGroupName,c.cmsRecipientGroupDescription,d.cmsRecipientTypeID,d.cmsRecipientTypeName
		,d.cmsRecipientTypeDescription,e.LastName as AuthorLastName,e.Firstname as AuthorFirstname ,e.MiddleName as AuthorMiddleName
		,f.LastName as RecipientLastName,f.FirstName as RecipientFirstName,f.MiddleName as RecipientMiddleName
		from dbo.CMS_ContentMessage a
		INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
		LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
		LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
		INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
		LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
		where a.cmsContentRecipientTypeID = 'R004'
		and a.cmsContentRecipientIndividualID = @individualId

		union

		select
		a.cmsContentID,a.cmsContentTitle,a.cmsContentBody,a.cmsContentAuthorID,a.cmsContentRecipientIndividualID,a.cmsContentRecipientSiteID
		,a.cmsContentDisplayDate,a.cmsContentExpireDate,a.cmsCreatedDate,b.cmsContentTypeID,b.cmsContentTypeName,b.cmsContentDescription
		,c.cmsRecipientGroupID,c.cmsRecipientGroupName,c.cmsRecipientGroupDescription,d.cmsRecipientTypeID,d.cmsRecipientTypeName
		,d.cmsRecipientTypeDescription,e.LastName as AuthorLastName,e.Firstname as AuthorFirstname ,e.MiddleName as AuthorMiddleName
		,f.LastName as RecipientLastName,f.FirstName as RecipientFirstName,f.MiddleName as RecipientMiddleName
		from dbo.CMS_ContentMessage a
		INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
		LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
		LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
		INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
		LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
		where a.cmsContentRecipientTypeID = 'R005'
		and a.cmsContentRecipientGroupID = @groupId*/
	)
		
	select distinct *
	from tbl
	where @currDt between tbl.cmsContentDisplayDate and tbl.cmsContentExpireDate
	order by tbl.cmsCreatedDate desc;
END
GO
/****** Object:  StoredProcedure [dbo].[GetCms_IndividualsMessagesById]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 14 May 14
-- Description:	Gets all messages for an Individual within the valid date range.
-- =============================================
CREATE PROCEDURE [dbo].[GetCms_IndividualsMessagesById]
	-- Add the parameters for the stored procedure here
	@individualId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @currDt datetime = DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()));
	
	SELECT a.cmsContentID
	,a.cmsContentTitle
	,a.cmsContentBody
	,a.cmsContentAuthorID
	,a.cmsContentRecipientIndividualID
	,a.cmsContentRecipientSiteID
	,a.cmsContentDisplayDate
	,a.cmsContentExpireDate
	,b.cmsContentTypeID 
	,b.cmsContentTypeName
	,b.cmsContentDescription
	,c.cmsRecipientGroupID
	,c.cmsRecipientGroupName
	,c.cmsRecipientGroupDescription
	,d.cmsRecipientTypeID
	,d.cmsRecipientTypeName
	,d.cmsRecipientTypeDescription
	,e.LastName as AuthorLastName
	,e.Firstname as AuthorFirstname 
	,e.MiddleName as AuthorMiddleName
	,f.LastName as RecipientLastName
	,f.FirstName as RecipientFirstName
	,f.MiddleName as RecipientMiddleName
	,cmsCreatedDate
	from dbo.CMS_ContentMessage a
	INNER JOIN dbo.CMS_ContentType b ON a.cmsContentTypeID = b.cmsContentTypeID
	LEFT OUTER JOIN dbo.CMS_RecipientGroup c ON a.cmsContentRecipientGroupID = c.cmsRecipientGroupID
	LEFT OUTER JOIN dbo.CMS_RecipientType d ON a.cmsContentRecipientTypeID = d.cmsRecipientTypeID
	INNER JOIN dbo.Individual e on a.cmsContentAuthorID = e.ID
	LEFT OUTER JOIN dbo.Individual f on a.cmsContentRecipientIndividualID = f.ID
	where a.cmsContentRecipientTypeID = 'R004'
	and a.cmsContentRecipientIndividualID = @individualId
	and @currDt between a.cmsContentDisplayDate and a.cmsContentExpireDate
	ORDER BY a.cmsCreatedDate desc;
END
GO
/****** Object:  StoredProcedure [dbo].[GetCMS_ContentByAuthorId]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 9 May 14
-- Description:	This SP is used to get all message titles and Id's associated with an author
-- =============================================
CREATE PROCEDURE [dbo].[GetCMS_ContentByAuthorId]
	-- Add the parameters for the stored procedure here
	@authorId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT c.cmsContentID, c.cmsContentTitle
	FROM CMS_ContentMessage c
	WHERE c.cmsContentAuthorID = @authorId;
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameItemByFrameCode]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFrameItemByFrameCode] 
@FrameCode	VARCHAR(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
			
	Select b.TypeEntry, a.IsActive, a.ModifiedBy, a.DateLastModified,
		b.Value, a.[Text], b.FrameCode, b.EligibiityFrameItem
	From dbo.FrameItemEligibilityFrameItemUnion b
		inner join dbo.FrameItem a on b.TypeEntry = a.TypeEntry
			and a.Value = b.Value
	where b.FrameCode = @FrameCode and a.IsActive = 1 and
		(a.TypeEntry = 'eye' OR a.TypeEntry = 'lens_type' OR
		a.TypeEntry = 'color' OR a.TypeEntry = 'bridge' OR
		a.TypeEntry = 'temple' OR a.TypeEntry = 'material' OR
		a.TypeEntry = 'color' OR a.TypeEntry = 'tint' OR
		a.TypeEntry = 'coating')

	Order by b.TypeEntry

	--SELECT 
	--	a.TypeEntry, 
	--	a.IsActive, 
	--	a.ModifiedBy, 
	--	a.DateLastModified, 
 --       a.[Value],
 --       a.[Text],
 --       b.FrameCode,
 --       b.EligibiityFrameItem
	--FROM dbo.FrameItem a
	--INNER JOIN dbo.FrameItemEligibilityFrameItemUnion b ON a.Value = b.Value
	--WHERE b.FrameCode = @FrameCode AND 
	--(a.TypeEntry = 'eye' OR
	--a.TypeEntry = 'lens_type' OR
	--a.TypeEntry = 'color' OR
	--a.TypeEntry = 'bridge' OR
	--a.TypeEntry = 'temple' OR
	--a.TypeEntry = 'material' OR
	--a.TypeEntry = 'color' OR
	--a.TypeEntry = 'tint')
	--ORDER BY a.TypeEntry	
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualIdByUserName]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualIdByUserName]
@userName varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT i.ID
    FROM Individual i
    INNER JOIN aspnet_Users u on u.UserId = i.aspnet_UserID
    WHERE u.LoweredUserName = LOWER(@userName) and i.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualIdByAspnetUserName]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualIdByAspnetUserName]
	@aspNetUserName varchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT i.ID
	FROM dbo.Individual i
	INNER JOIN dbo.aspnet_Users u on i.aspnet_UserID = u.UserId
	WHERE u.LoweredUserName = LOWER(@aspNetUserName);
END
GO
/****** Object:  StoredProcedure [dbo].[GetLMTP]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 26 July 2018
-- Description:	Use to retrieve LMTP Records for a lab/Clinic combination
-- MODIFIED:  5/22/19 - lb - modified where clauses to remove clinicsitecode
-- =============================================
CREATE PROCEDURE [dbo].[GetLMTP]
	-- Add the parameters for the stored procedure here
	@LabSite VARCHAR(6),
	--@ClinicSite VARCHAR(6) = Null,
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID INT = 0,
		@RecCnt  INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.LMTP
	WHERE LabSiteCode = @LabSite
	--WHERE LabSiteCode = @LabSite AND (ClinicSiteCode IS NULL OR ClinicSiteCode = @ClinicSite)
		
    BEGIN TRY
		IF @RecCnt = 1
		BEGIN
			SELECT * FROM dbo.LMTP 
			WHERE LabSiteCode = @LabSite
			--WHERE LabSiteCode = @LabSite AND ClinicSiteCode = @ClinicSite
			SET @Success = 1
		END
		ELSE IF @RecCnt > 1
		BEGIN
			SELECT * FROM dbo.LMTP WHERE LabSiteCode = @LabSite
			SET @Success = 1
		END
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN
				-- Select Failed
				SET @Success = 0
			END		
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT		
	END Catch
END
GO
/****** Object:  StoredProcedure [dbo].[GetFramesAndItemsByEligibility]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  09/17/2015 - BAF - to utilize the 
--    FrameRestrictions table 
--		12/13/2017 - baf - to allow other downed pilots
--			ability to order AFF and AFJS Frames
-- =============================================
CREATE PROCEDURE [dbo].[GetFramesAndItemsByEligibility] 
@Demographic	VARCHAR(8),
@SiteCode		VARCHAR(6)
AS
BEGIN
	DECLARE @DemoChanged VARCHAR(7),
		@BOS VarChar(1), @Priority VarChar(1)
		
	SELECT @DemoChanged = SUBSTRING(@Demographic, 1,6) + 'B', @BOS = SUBSTRING(@Demographic,4,1), 
		@Priority = SUBSTRING(@Demographic,8,1)
Print @DemoChanged + '  -  ' + @BOS + ' ' + @Priority

If @BOS = 'F'
	Begin
	SELECT * FROM 
	(
		SELECT
		DISTINCT(a.FrameCode),
		a.FrameDescription,
		a.FrameCode + ' - ' + a.FrameDescription as FrameLongDescription,
		a.FrameNotes,
		a.IsInsert,
		a.MaxPair, 
		d.TypeEntry,
		d.Value,
		d.[Text],
		a.IsFOC
		FROM dbo.Frame a
		INNER JOIN dbo.FrameEligibilityFrameUnion b ON a.FrameCode = b.FrameCode
		INNER JOIN dbo.FrameItemEligibilityFrameItemUnion c ON b.FrameCode = c.FrameCode
		INNER JOIN dbo.FrameItem d ON c.Value = d.Value AND c.TypeEntry = d.TypeEntry
		Inner Join (Select * from dbo.FrameRestrictions where SiteCode = @SiteCode or SiteCode = 'NONE' and SiteType = 'CLINIC') fr
			 ON a.FrameCode = fr.FrameCode
		WHERE (b.EligibilityFrame = SUBSTRING(@Demographic, 1, 7) OR
		b.EligibilityFrame = @DemoChanged) AND 
		c.FrameCode = a.FrameCode AND
		(c.EligibiityFrameItem = SUBSTRING(@Demographic, 7, 2) OR 
		c.EligibiityFrameItem = 'B' + SUBSTRING(@Demographic, 8, 1)) AND 
		a.IsActive = 1 AND 	d.IsActive = 1
		) x
		ORDER BY x.FrameCode, x.TypeEntry
	End
ELSE 
	Begin 
		SELECT * FROM 
		(
		SELECT
		DISTINCT
		(a.FrameCode),
		a.FrameDescription,
		a.FrameCode + ' - ' + a.FrameDescription as FrameLongDescription,
		a.FrameNotes,
		a.IsInsert,
		a.MaxPair, 
		d.TypeEntry,
		d.Value,
		d.[Text],
		a.IsFOC
		FROM dbo.Frame a
		INNER JOIN dbo.FrameEligibilityFrameUnion b ON a.FrameCode = b.FrameCode
		INNER JOIN dbo.FrameItemEligibilityFrameItemUnion c ON b.FrameCode = c.FrameCode
		INNER JOIN dbo.FrameItem d ON c.Value = d.Value AND c.TypeEntry = d.TypeEntry
		Inner Join (Select * from dbo.FrameRestrictions where SiteCode = @SiteCode or SiteCode = 'NONE' and SiteType = 'CLINIC') fr
			 ON a.FrameCode = fr.FrameCode
		WHERE (b.EligibilityFrame = SUBSTRING(@Demographic, 1, 7) OR
		b.EligibilityFrame = @DemoChanged) AND 
		c.FrameCode = a.FrameCode AND
		(c.EligibiityFrameItem = SUBSTRING(@Demographic, 7, 2) OR 
		c.EligibiityFrameItem = 'B' + SUBSTRING(@Demographic, 8, 1)) AND 
		a.IsActive = 1 AND 	d.IsActive = 1
		and (@Priority <> 'S' or (a.FrameCode not in ('AFF','AFJS') and @Priority = 'S'))
		) x
		ORDER BY x.FrameCode, x.TypeEntry
	
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetFrameItemsByFrameCodeAndEligibility]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		<Author,,Name>
 Create date: <Create Date,,>
 Description:	<Description,,>
 Modified:  27 May 2016 - baf - TO include the availability column
			for tints being pulled.
		7 Apr 2017 - baf - Modified to pull only available
			Frame Items
		12 June 2017 - baf - added coating
		2 January 2018 - baf - Modified for Multiple Coatings
 =============================================*/
CREATE PROCEDURE [dbo].[GetFrameItemsByFrameCodeAndEligibility] 
@FrameCode			VARCHAR(15),
@Demographic		VARCHAR(8)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--CREATE TABLE #FrameItems
	--(	FrameCode VARCHAR(10),
	--	TypeEntry VarChar(40),
	--	Value VarChar(40))
		
	--INSERT INTO #FrameItems
	--SELECT FrameCode, TypeENtry, Value FROM dbo.FrameItemEligibilityFrameItemUnion
	--	WHERE Availability = 0
		
		
	
	DECLARE @DemoChanged VARCHAR(8)	
	SELECT @DemoChanged = 'B' + SUBSTRING(@Demographic, 8, 1)
	
	SELECT 
		b.TypeEntry, 
		a.IsActive, 
		a.ModifiedBy, 
		a.DateLastModified, 
        b.[Value],
        a.[Text],
        a.Availability	
	FROM dbo.FrameItem a
	INNER JOIN dbo.FrameItemEligibilityFrameItemUnion b ON a.Value = b.Value AND a.TypeEntry = b.TypeEntry
	WHERE (b.EligibiityFrameItem = SUBSTRING(@Demographic, 7, 2) OR b.EligibiityFrameItem = @DemoChanged) AND
	b.FrameCode = @FrameCode AND 
	(b.TypeEntry = 'eye' OR
	b.TypeEntry = 'lens_type' OR
	b.TypeEntry = 'color' OR
	b.TypeEntry = 'bridge'  OR
	b.TypeEntry = 'temple' OR
	b.TypeEntry = 'material' OR
	b.TypeEntry = 'color' OR
	a.TypeEntry = 'tint' OR
	a.TypeEntry = 'coating')
	--AND a.Value NOT IN (SELECT Value FROM #FrameItems WHERE FrameCode = @FrameCode)
	AND a.IsActive = 1
	ORDER BY b.TypeEntry
	
	--DROP TABLE #FrameItems

End
GO
/****** Object:  StoredProcedure [dbo].[GetFramesBySiteBOSPriority]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 20 June 2017
-- Description:	Retrieves the Frames for Site Preferences
-- Modified:  1/30/18 - baf - To include VA
-- =============================================
CREATE PROCEDURE [dbo].[GetFramesBySiteBOSPriority]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@Priority CHAR(1)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @BOS CHAR(1)
		
	SELECT @BOS = BOS FROM dbo.SiteCode WHERE SiteCode = @SiteCode
	
	IF @BOS = 'C'
	BEGIN
		SELECT DISTINCT fr.FrameCode + ' - ' + fr.FrameDescription 
			AS FrameLongDescription, fr.FrameCode
		FROM dbo.Frame fr
		INNER JOIN dbo.FrameEligibilityFrameUnion fefu ON fr.FrameCode = fefu.FrameCode
		INNER JOIN dbo.FrameItemEligibilityFrameItemUnion fief ON fief.framecode = fr.FrameCode
		WHERE SUBSTRING(fefu.EligibilityFrame,4,1) = @BOS
			AND RIGHT(fief.EligibiityFrameItem,1) = @Priority
			AND fr.IsActive = 1
		ORDER BY fr.FrameCode
	END
	ELSE
	IF @BOS <> 'V'
	BEGIN
		SELECT DISTINCT fr.FrameCode + ' - ' + fr.FrameDescription 
			AS FrameLongDescription, fr.FrameCode
		FROM dbo.Frame fr
		INNER JOIN dbo.FrameEligibilityFrameUnion fefu ON fr.FrameCode = fefu.FrameCode
		INNER JOIN dbo.FrameItemEligibilityFrameItemUnion fief ON fief.framecode = fr.FrameCode
		WHERE SUBSTRING(fefu.EligibilityFrame,4,1) = @BOS
			AND RIGHT(fief.EligibiityFrameItem,1) = @Priority	
			AND Fr.FrameCode <> 'A1000'
			AND fr.IsActive = 1
		ORDER BY Fr.FrameCode
	END
	IF @BOS = 'V'
	BEGIN
		SELECT DISTINCT fr.FrameCode + ' - ' + fr.FrameDescription
			AS FrameLongDescription, fr.FrameCode
		FROM dbo.Frame fr
		INNER JOIN dbo.FrameEligibilityFrameUnion fefu ON fr.FrameCode = fefu.FrameCode
		INNER JOIN dbo.FrameItemEligibilityFrameItemUnion fief ON fief.framecode = fr.FrameCode
		WHERE --SUBSTRING(fefu.EligibilityFrame,4,1) = @BOS  Removed for VA
			RIGHT(fief.EligibiityFrameItem,1) = @Priority	
			AND Fr.FrameCode <> 'A1000'
			AND fr.IsActive = 1
		ORDER BY Fr.FrameCode		
	END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_SetProperties]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Profile_SetProperties]
    @ApplicationName        nvarchar(256),
    @PropertyNames          ntext,
    @PropertyValuesString   ntext,
    @PropertyValuesBinary   image,
    @UserName               nvarchar(256),
    @IsUserAnonymous        bit,
    @CurrentTimeUtc         datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
       BEGIN TRANSACTION
       SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DECLARE @UserId uniqueidentifier
    DECLARE @LastActivityDate datetime
    SELECT  @UserId = NULL
    SELECT  @LastActivityDate = @CurrentTimeUtc

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)
    --IF (@UserId IS NULL)
    --    EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, @IsUserAnonymous, @LastActivityDate, @UserId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Users
    SET    LastActivityDate=@CurrentTimeUtc
    WHERE  UserId = @UserId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS( SELECT *
               FROM   dbo.aspnet_Profile
               WHERE  UserId = @UserId))
        UPDATE dbo.aspnet_Profile
        SET    PropertyNames=@PropertyNames, PropertyValuesString = @PropertyValuesString,
               PropertyValuesBinary = @PropertyValuesBinary, LastUpdatedDate=@CurrentTimeUtc
        WHERE  UserId = @UserId
    ELSE
        INSERT INTO dbo.aspnet_Profile(UserId, PropertyNames, PropertyValuesString, PropertyValuesBinary, LastUpdatedDate)
             VALUES (@UserId, @PropertyNames, @PropertyValuesString, @PropertyValuesBinary, @CurrentTimeUtc)

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetProperties]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Profile_GetProperties]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)

    IF (@UserId IS NULL)
        RETURN
    SELECT TOP 1 PropertyNames, PropertyValuesString, PropertyValuesBinary
    FROM         dbo.aspnet_Profile
    WHERE        UserId = @UserId

    IF (@@ROWCOUNT > 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate=@CurrentTimeUtc
        WHERE  UserId = @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetProfiles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Profile_GetProfiles]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @PageIndex              int,
    @PageSize               int,
    @UserNameToMatch        nvarchar(256) = NULL,
    @InactiveSinceDate      datetime      = NULL
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT  u.UserId
        FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
        WHERE   ApplicationId = @ApplicationId
            AND u.UserId = p.UserId
            AND (@InactiveSinceDate IS NULL OR LastActivityDate <= @InactiveSinceDate)
            AND (     (@ProfileAuthOptions = 2)
                   OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                   OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                 )
            AND (@UserNameToMatch IS NULL OR LoweredUserName LIKE LOWER(@UserNameToMatch))
        ORDER BY UserName

    SELECT  u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate,
            DATALENGTH(p.PropertyNames) + DATALENGTH(p.PropertyValuesString) + DATALENGTH(p.PropertyValuesBinary)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p, #PageIndexForUsers i
    WHERE   u.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= @PageLowerBound AND i.IndexId <= @PageUpperBound

    SELECT COUNT(*)
    FROM   #PageIndexForUsers

    DROP TABLE #PageIndexForUsers
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @InactiveSinceDate      datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT 0
        RETURN
    END

    SELECT  COUNT(*)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
    WHERE   ApplicationId = @ApplicationId
        AND u.UserId = p.UserId
        AND (LastActivityDate <= @InactiveSinceDate)
        AND (
                (@ProfileAuthOptions = 2)
                OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
            )
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_RoleExists]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Roles_RoleExists]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(0)
    IF (EXISTS (SELECT RoleName FROM dbo.aspnet_Roles WHERE LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId ))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_GetAllRoles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Roles_GetAllRoles] (
    @ApplicationName           nvarchar(256))
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN
    SELECT RoleName
    FROM   dbo.aspnet_Roles WHERE ApplicationId = @ApplicationId
    ORDER BY RoleName
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Paths_CreatePath]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Paths_CreatePath]
    @ApplicationId UNIQUEIDENTIFIER,
    @Path           NVARCHAR(256),
    @PathId         UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    BEGIN TRANSACTION
    IF (NOT EXISTS(SELECT * FROM dbo.aspnet_Paths WHERE LoweredPath = LOWER(@Path) AND ApplicationId = @ApplicationId))
    BEGIN
        INSERT dbo.aspnet_Paths (ApplicationId, Path, LoweredPath) VALUES (@ApplicationId, @Path, LOWER(@Path))
    END
    COMMIT TRANSACTION
    SELECT @PathId = PathId FROM dbo.aspnet_Paths WHERE LOWER(@Path) = LoweredPath AND ApplicationId = @ApplicationId
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_Rx]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10 July 2017
-- Description:	Allows deletion of a SitePref_Rx records
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSitePref_Rx]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @Cnt int = 0
	
	SELECT @Cnt = COUNT(*) FROM dbo.SitePref_Rx
	WHERE SiteCode = @SiteCode 
	
	IF @Cnt = 1 
	BEGIN
		DELETE FROM dbo.SitePref_Rx WHERE
			SiteCode = @SiteCode 
	End
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_Orders]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10 July 2017
-- Description:	Allows deletion of a SitePref_Orders records
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSitePref_Orders]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6),
	@Priority CHAR(1)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @Cnt int = 0
	
	SELECT @Cnt = COUNT(*) FROM dbo.SitePref_Orders
	WHERE SiteCode = @SiteCode AND [Priority] = @Priority
	
	IF @Cnt = 1 
	BEGIN
		DELETE FROM dbo.SitePref_Orders WHERE
			SiteCode = @SiteCode AND [Priority] = @Priority
	End
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_LabMailToPatient]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhasen
-- Create date: 04 April 2019
-- Description:	Use to Delete Lab MailToPatient Preferences
-- =============================================
Create PROCEDURE [dbo].[DeleteSitePref_LabMailToPatient]
	-- Add the parameters for the stored procedure here
	@LabSiteCode VarChar(6),
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_LabMailToPatient
	WHERE LabSiteCode = @LabSiteCode
	
	BEGIN TRY
		IF @RecCnt = 1
		BEGIN
			BEGIN TRANSACTION
				Delete from dbo.SitePref_LabMailToPatient where LabSiteCode = @LabSiteCode
			COMMIT TRANSACTION
		END

		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_Lab]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhasen
-- Create date: 18 July 2018
-- Description:	Use to Delete Lab Site Preferences
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSitePref_Lab]
	-- Add the parameters for the stored procedure here
	@LabSiteCode VarChar(6),
	@Success BIT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @RecCnt INT = 0,
		@ErrorLogID INT = 0
		
	SELECT @RecCnt = COUNT(*) FROM dbo.SitePref_Labs
	WHERE LabSiteCode = @LabSiteCode
	
	BEGIN TRY
		IF @RecCnt = 1
		BEGIN
			BEGIN TRANSACTION
				Delete from dbo.SitePref_Labs where LabSiteCode = @LabSiteCode
			COMMIT TRANSACTION
		END

		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH	
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_Justifications]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 August 2017
-- Description:	Deletes Justification records for a site
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSitePref_Justifications]
	
	@SiteCode VarChar(6),
	@Rsn VARCHAR(50) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF @Rsn IS NULL -- Delete all for this site
		Begin
			DELETE FROM dbo.SitePref_Justifications 
			WHERE SiteCode = @SiteCode
		END
	ELSE
		BEGIN
			DELETE FROM dbo.SitePref_Justifications
			WHERE SiteCode = @SiteCode AND Reason = @Rsn
		END
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_General]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10 July 2017
-- Description:	Allows deletion of a SitePref_General records
-- =============================================
CREATE PROCEDURE [dbo].[DeleteSitePref_General]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @Cnt int = 0
	
	SELECT @Cnt = COUNT(*) FROM dbo.SitePref_General
	WHERE SiteCode = @SiteCode 
	
	IF @Cnt = 1 
	BEGIN
		DELETE FROM dbo.SitePref_General WHERE
			SiteCode = @SiteCode 
	End
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteSitePref_Frames]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10 July 2017
-- Description:	Allows deletion of a SitePref_Frame records
-- =============================================
Create PROCEDURE [dbo].[DeleteSitePref_Frames]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6),
	@FrameCode VARCHAR(15)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @Cnt int = 0
	
	SELECT @Cnt = COUNT(*) FROM dbo.SitePref_Frames
	WHERE SiteCode = @SiteCode AND Frame = @FrameCode
	
	IF @Cnt = 1 
	BEGIN
		DELETE FROM dbo.SitePref_Frames WHERE
			SiteCode = @SiteCode and Frame = @FrameCode
	End
END
GO
/****** Object:  StoredProcedure [dbo].[CMSUpdateMessage]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CMSUpdateMessage] 
@ContentID				int,
@ContentTitle			varchar(500),
@ContentBody			varchar(1024),
@ContentTypeID			varchar(5),
@AuthorID				int,
@RecipientTypeID		varchar(5),
@RecipientIndividualID	int = null,
@RecipientSiteID		varchar(6),
@RecipientGroupID		varchar(5) = null,
@DisplayDate			datetime,
@ExpireDate				datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int;
	SET @ErrorLogID = 0;
	
    BEGIN TRY
		BEGIN TRANSACTION
			
		-- Insert statements for procedure here
		UPDATE dbo.CMS_ContentMessage SET
		cmsContentTitle = @ContentTitle,
		cmsContentBody = @ContentBody,
		cmsContentTypeID = @ContentTypeID,
		cmsContentAuthorID = @AuthorID,
		cmsContentRecipientTypeID = @RecipientTypeID,
		cmsContentRecipientIndividualID = @RecipientIndividualID,
		cmsContentRecipientSiteID = @RecipientSiteID,
		cmsContentRecipientGroupID = @RecipientGroupID,
		cmsContentDisplayDate = @DisplayDate,
		cmsContentExpireDate = @ExpireDate,
		cmsCreatedDate = GETDATE()
		WHERE cmsContentID = @ContentID
    	
    	COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
    	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN

    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[CMSInsertMessage]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CMSInsertMessage] 
@ContentTitle			varchar(500),
@ContentBody			varchar(1024),
@ContentTypeID			varchar(5),
@AuthorID				int,
@RecipientTypeID		varchar(5),
@RecipientIndividualID	int = null,
@RecipientSiteID		varchar(6) = null,
@RecipientGroupID		varchar(5) = null,
@DisplayDate			datetime,
@ExpireDate				datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
DECLARE @ErrorLogID varchar
	
BEGIN TRY
	BEGIN TRANSACTION

    -- Insert statements for procedure here
    INSERT INTO dbo.CMS_ContentMessage
    (
		cmsContentTitle, 
		cmsContentBody, 
		cmsContentTypeID, 
		cmsContentAuthorID, 
		cmsContentRecipientTypeID, 
		cmsContentRecipientIndividualID,
		cmsContentRecipientSiteID, 
		cmsContentRecipientGroupID, 
		cmsContentDisplaydate, 
		cmsContentExpireDate,
		cmsCreatedDate
	)
    VALUES
    (
		@ContentTitle, 
		@ContentBody, 
		@ContentTypeID, 
		@AuthorID, 
		@RecipientTypeID, 
		@RecipientIndividualID, 
		@RecipientSiteID,
		@RecipientGroupID,
		@DisplayDate, 
		@ExpireDate,
		GETDATE()
	)
	
	COMMIT TRANSACTION

END TRY
BEGIN CATCH
   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	RETURN
END CATCH


END
GO
/****** Object:  StoredProcedure [dbo].[CMSDeleteMessage]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CMSDeleteMessage]
	-- Add the parameters for the stored procedure here
@ContentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	DECLARE @ErrorLogID int;
	SET @ErrorLogID = 0;
	
    BEGIN TRY
		BEGIN TRANSACTION
			
			DELETE FROM dbo.CMS_ContentMessage
			WHERE cmsContentID = @ContentId;
			
		COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
    	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN

    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Users_CreateUser]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Users_CreateUser]
    @ApplicationId    uniqueidentifier,
    @UserName         nvarchar(256),
    @IsUserAnonymous  bit,
    @LastActivityDate DATETIME,
    @UserId           uniqueidentifier OUTPUT
AS
BEGIN
    IF( @UserId IS NULL )
        SELECT @UserId = NEWID()
    ELSE
    BEGIN
        IF( EXISTS( SELECT UserId FROM dbo.aspnet_Users
                    WHERE @UserId = UserId ) )
            RETURN -1
    END

    INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
    VALUES (@ApplicationId, @UserId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate)
    

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_CreateRole]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Roles_CreateRole]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS(SELECT RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId))
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    INSERT INTO dbo.aspnet_Roles
                (ApplicationId, RoleName, LoweredRoleName)
         VALUES (@ApplicationId, @RoleName, LOWER(@RoleName))

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_DeleteInactiveProfiles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Profile_DeleteInactiveProfiles]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @InactiveSinceDate      datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT  0
        RETURN
    END

    DELETE
    FROM    dbo.aspnet_Profile
    WHERE   UserId IN
            (   SELECT  UserId
                FROM    dbo.aspnet_Users u
                WHERE   ApplicationId = @ApplicationId
                        AND (LastActivityDate <= @InactiveSinceDate)
                        AND (
                                (@ProfileAuthOptions = 2)
                             OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                             OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                            )
            )

    SELECT  @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteLensStock]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 February 2017
-- Description:	This sp deletes a specific record from
--		dbo.LensStock
-- =============================================
CREATE PROCEDURE [dbo].[DeleteLensStock]
	-- Add the parameters for the stored procedure here
	@ID INT,
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID INT = 0

	BEGIN TRY
		Begin Transaction
			Update dbo.LensStock set ISActive = 0 where ID = @ID
--			DELETE FROM dbo.LensStock WHERE ID = @ID

		Set @Success = 1
		Commit Transaction
	END TRY
	BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			SET @Success = 0
			RETURN
	END CATCH
	END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles]
	@ApplicationName  nvarchar(256),
	@UserNames		  nvarchar(4000),
	@RoleNames		  nvarchar(4000)
AS
BEGIN
	DECLARE @AppId uniqueidentifier
	SELECT  @AppId = NULL
	SELECT  @AppId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
	IF (@AppId IS NULL)
		RETURN(2)


	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	DECLARE @tbNames  table(Name nvarchar(256) NOT NULL PRIMARY KEY)
	DECLARE @tbRoles  table(RoleId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @tbUsers  table(UserId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @Num	  int
	DECLARE @Pos	  int
	DECLARE @NextPos  int
	DECLARE @Name	  nvarchar(256)
	DECLARE @CountAll int
	DECLARE @CountU	  int
	DECLARE @CountR	  int


	SET @Num = 0
	SET @Pos = 1
	WHILE(@Pos <= LEN(@RoleNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @RoleNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@RoleNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@RoleNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbRoles
	  SELECT RoleId
	  FROM   dbo.aspnet_Roles ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredRoleName AND ar.ApplicationId = @AppId
	SELECT @CountR = @@ROWCOUNT

	IF (@CountR <> @Num)
	BEGIN
		SELECT TOP 1 N'', Name
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT ar.LoweredRoleName FROM dbo.aspnet_Roles ar,  @tbRoles r WHERE r.RoleId = ar.RoleId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(2)
	END


	DELETE FROM @tbNames WHERE 1=1
	SET @Num = 0
	SET @Pos = 1


	WHILE(@Pos <= LEN(@UserNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @UserNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@UserNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@UserNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbUsers
	  SELECT UserId
	  FROM   dbo.aspnet_Users ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredUserName AND ar.ApplicationId = @AppId

	SELECT @CountU = @@ROWCOUNT
	IF (@CountU <> @Num)
	BEGIN
		SELECT TOP 1 Name, N''
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT au.LoweredUserName FROM dbo.aspnet_Users au,  @tbUsers u WHERE u.UserId = au.UserId)

		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(1)
	END

	SELECT  @CountAll = COUNT(*)
	FROM	dbo.aspnet_UsersInRoles ur, @tbUsers u, @tbRoles r
	WHERE   ur.UserId = u.UserId AND ur.RoleId = r.RoleId

	IF (@CountAll <> @CountU * @CountR)
	BEGIN
		SELECT TOP 1 UserName, RoleName
		FROM		 @tbUsers tu, @tbRoles tr, dbo.aspnet_Users u, dbo.aspnet_Roles r
		WHERE		 u.UserId = tu.UserId AND r.RoleId = tr.RoleId AND
					 tu.UserId NOT IN (SELECT ur.UserId FROM dbo.aspnet_UsersInRoles ur WHERE ur.RoleId = tr.RoleId) AND
					 tr.RoleId NOT IN (SELECT ur.RoleId FROM dbo.aspnet_UsersInRoles ur WHERE ur.UserId = tu.UserId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(3)
	END

	DELETE FROM dbo.aspnet_UsersInRoles
	WHERE UserId IN (SELECT UserId FROM @tbUsers)
	  AND RoleId IN (SELECT RoleId FROM @tbRoles)
	IF( @TranStarted = 1 )
		COMMIT TRANSACTION
	RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_IsUserInRole]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_IsUserInRole]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(2)
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    DECLARE @RoleId uniqueidentifier
    SELECT  @RoleId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(2)

    SELECT  @RoleId = RoleId
    FROM    dbo.aspnet_Roles
    WHERE   LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
        RETURN(3)

    IF (EXISTS( SELECT * FROM dbo.aspnet_UsersInRoles WHERE  UserId = @UserId AND RoleId = @RoleId))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_GetUsersInRoles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_GetUsersInRoles]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId uniqueidentifier
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId
    ORDER BY u.UserName
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_GetRolesForUser]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_GetRolesForUser]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(1)

    SELECT r.RoleName
    FROM   dbo.aspnet_Roles r, dbo.aspnet_UsersInRoles ur
    WHERE  r.RoleId = ur.RoleId AND r.ApplicationId = @ApplicationId AND ur.UserId = @UserId
    ORDER BY r.RoleName
    RETURN (0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_FindUsersInRole]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_FindUsersInRole]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256),
    @UserNameToMatch  nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId uniqueidentifier
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId AND LoweredUserName LIKE LOWER(@UserNameToMatch)
    ORDER BY u.UserName
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_UsersInRoles_AddUsersToRoles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_AddUsersToRoles]
	@ApplicationName  nvarchar(256),
	@UserNames		  nvarchar(4000),
	@RoleNames		  nvarchar(4000),
	@CurrentTimeUtc   datetime
AS
BEGIN
	DECLARE @AppId uniqueidentifier
	SELECT  @AppId = NULL
	SELECT  @AppId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
	IF (@AppId IS NULL)
		RETURN(2)
	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	DECLARE @tbNames	table(Name nvarchar(256) NOT NULL PRIMARY KEY)
	DECLARE @tbRoles	table(RoleId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @tbUsers	table(UserId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @Num		int
	DECLARE @Pos		int
	DECLARE @NextPos	int
	DECLARE @Name		nvarchar(256)

	SET @Num = 0
	SET @Pos = 1
	WHILE(@Pos <= LEN(@RoleNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @RoleNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@RoleNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@RoleNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbRoles
	  SELECT RoleId
	  FROM   dbo.aspnet_Roles ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredRoleName AND ar.ApplicationId = @AppId

	IF (@@ROWCOUNT <> @Num)
	BEGIN
		SELECT TOP 1 Name
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT ar.LoweredRoleName FROM dbo.aspnet_Roles ar,  @tbRoles r WHERE r.RoleId = ar.RoleId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(2)
	END

	DELETE FROM @tbNames WHERE 1=1
	SET @Num = 0
	SET @Pos = 1

	WHILE(@Pos <= LEN(@UserNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @UserNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@UserNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@UserNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbUsers
	  SELECT UserId
	  FROM   dbo.aspnet_Users ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredUserName AND ar.ApplicationId = @AppId

	IF (@@ROWCOUNT <> @Num)
	BEGIN
		DELETE FROM @tbNames
		WHERE LOWER(Name) IN (SELECT LoweredUserName FROM dbo.aspnet_Users au,  @tbUsers u WHERE au.UserId = u.UserId)

		INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
		  SELECT @AppId, NEWID(), Name, LOWER(Name), 0, @CurrentTimeUtc
		  FROM   @tbNames

		INSERT INTO @tbUsers
		  SELECT  UserId
		  FROM	dbo.aspnet_Users au, @tbNames t
		  WHERE   LOWER(t.Name) = au.LoweredUserName AND au.ApplicationId = @AppId
	END

	IF (EXISTS (SELECT * FROM dbo.aspnet_UsersInRoles ur, @tbUsers tu, @tbRoles tr WHERE tu.UserId = ur.UserId AND tr.RoleId = ur.RoleId))
	BEGIN
		SELECT TOP 1 UserName, RoleName
		FROM		 dbo.aspnet_UsersInRoles ur, @tbUsers tu, @tbRoles tr, aspnet_Users u, aspnet_Roles r
		WHERE		u.UserId = tu.UserId AND r.RoleId = tr.RoleId AND tu.UserId = ur.UserId AND tr.RoleId = ur.RoleId

		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(3)
	END

	INSERT INTO dbo.aspnet_UsersInRoles (UserId, RoleId)
	SELECT UserId, RoleId
	FROM @tbUsers, @tbRoles

	IF( @TranStarted = 1 )
		COMMIT TRANSACTION
	RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Users_DeleteUser]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Users_DeleteUser]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @TablesToDeleteFrom int,
    @NumTablesDeletedFrom int OUTPUT
AS
BEGIN
    DECLARE @UserId               uniqueidentifier
    SELECT  @UserId               = NULL
    SELECT  @NumTablesDeletedFrom = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    DECLARE @ErrorCode   int
    DECLARE @RowCount    int

    SET @ErrorCode = 0
    SET @RowCount  = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   u.LoweredUserName       = LOWER(@UserName)
        AND u.ApplicationId         = a.ApplicationId
        AND LOWER(@ApplicationName) = a.LoweredApplicationName

    IF (@UserId IS NULL)
    BEGIN
        GOTO Cleanup
    END

    -- Delete from Membership table if (@TablesToDeleteFrom & 1) is set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
    BEGIN
        DELETE FROM dbo.aspnet_Membership WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
               @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
    IF ((@TablesToDeleteFrom & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_UsersInRoles') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
    IF ((@TablesToDeleteFrom & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_Profile WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
    IF ((@TablesToDeleteFrom & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (@TablesToDeleteFrom & 2) <> 0 AND
        (@TablesToDeleteFrom & 4) <> 0 AND
        (@TablesToDeleteFrom & 8) <> 0 AND
        (EXISTS (SELECT UserId FROM dbo.aspnet_Users WHERE @UserId = UserId)))
    BEGIN
        DELETE FROM dbo.aspnet_Users WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:
    SET @NumTablesDeletedFrom = 0

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
	    ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[DeletePrescription]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 3/21/2016
-- Description:	This stored procedure allows for 
--        deleting(setting IsActive to 0) for a 
--        Prescription that has not been used on a Patient Order
-- =============================================
CREATE PROCEDURE [dbo].[DeletePrescription]
	-- Add the parameters for the stored procedure here
	@ModifiedBy	VARCHAR(200),
	@ID	INT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Prescription 
	SET IsActive = 0, ModifiedBy = @ModifiedBy, DateLastModified = GETDATE() 
	WHERE ID = @ID
	
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteProfilePrimarySiteUnion]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 Nov 2016
-- Description:	Delete a record from dbo.Profile_PrimarySiteUnion
-- =============================================
CREATE PROCEDURE [dbo].[DeleteProfilePrimarySiteUnion]

	@lowerusername VarChar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Delete from dbo.Profile_PrimarySiteUnion
	where LowerUserName = @lowerusername
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UpdateUserInfo]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UpdateUserInfo]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @IsPasswordCorrect              bit,
    @UpdateLastLoginActivityDate    bit,
    @MaxInvalidPasswordAttempts     int,
    @PasswordAttemptWindow          int,
    @CurrentTimeUtc                 datetime,
    @LastLoginDate                  datetime,
    @LastActivityDate               datetime
AS
BEGIN
    DECLARE @UserId                                 uniqueidentifier
    DECLARE @IsApproved                             bit
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId,
            @IsApproved = m.IsApproved,
            @IsLockedOut = m.IsLockedOut,
            @LastLockoutDate = m.LastLockoutDate,
            @FailedPasswordAttemptCount = m.FailedPasswordAttemptCount,
            @FailedPasswordAttemptWindowStart = m.FailedPasswordAttemptWindowStart,
            @FailedPasswordAnswerAttemptCount = m.FailedPasswordAnswerAttemptCount,
            @FailedPasswordAnswerAttemptWindowStart = m.FailedPasswordAnswerAttemptWindowStart
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m WITH ( UPDLOCK )
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF ( @@rowcount = 0 )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    IF( @IsLockedOut = 1 )
    BEGIN
        GOTO Cleanup
    END

    IF( @IsPasswordCorrect = 0 )
    BEGIN
        IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAttemptWindowStart ) )
        BEGIN
            SET @FailedPasswordAttemptWindowStart = @CurrentTimeUtc
            SET @FailedPasswordAttemptCount = 1
        END
        ELSE
        BEGIN
            SET @FailedPasswordAttemptWindowStart = @CurrentTimeUtc
            SET @FailedPasswordAttemptCount = @FailedPasswordAttemptCount + 1
        END

        BEGIN
            IF( @FailedPasswordAttemptCount >= @MaxInvalidPasswordAttempts )
            BEGIN
                SET @IsLockedOut = 1
                SET @LastLockoutDate = @CurrentTimeUtc
            END
        END
    END
    ELSE
    BEGIN
        IF( @FailedPasswordAttemptCount > 0 OR @FailedPasswordAnswerAttemptCount > 0 )
        BEGIN
            SET @FailedPasswordAttemptCount = 0
            SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            SET @FailedPasswordAnswerAttemptCount = 0
            SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )
        END
    END

    IF( @UpdateLastLoginActivityDate = 1 )
    BEGIN
        UPDATE  dbo.aspnet_Users
        SET     LastActivityDate = @LastActivityDate
        WHERE   @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END

        UPDATE  dbo.aspnet_Membership
        SET     LastLoginDate = @LastLoginDate
        WHERE   UserId = @UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END


    UPDATE dbo.aspnet_Membership
    SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
        FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
        FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
        FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
        FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
    WHERE @UserId = UserId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UpdateUser]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UpdateUser]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @Email                nvarchar(256),
    @Comment              ntext,
    @IsApproved           bit,
    @LastLoginDate        datetime,
    @LastActivityDate     datetime,
    @UniqueEmail          int,
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId, @ApplicationId = a.ApplicationId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)

    IF (@UniqueEmail = 1)
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership WITH (UPDLOCK, HOLDLOCK)
                    WHERE ApplicationId = @ApplicationId  AND @UserId <> UserId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            RETURN(7)
        END
    END

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    UPDATE dbo.aspnet_Users WITH (ROWLOCK)
    SET
         LastActivityDate = @LastActivityDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    UPDATE dbo.aspnet_Membership WITH (ROWLOCK)
    SET
         Email            = @Email,
         LoweredEmail     = LOWER(@Email),
         Comment          = @Comment,
         IsApproved       = @IsApproved,
         LastLoginDate    = @LastLoginDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN -1
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_UnlockUser]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UnlockUser]
    @ApplicationName                         nvarchar(256),
    @UserName                                nvarchar(256)
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
        RETURN 1

    UPDATE dbo.aspnet_Membership
    SET IsLockedOut = 0,
        FailedPasswordAttemptCount = 0,
        FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 ),
        FailedPasswordAnswerAttemptCount = 0,
        FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 ),
        LastLockoutDate = CONVERT( datetime, '17540101', 112 )
    WHERE @UserId = UserId

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_SetPassword]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_SetPassword]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @NewPassword      nvarchar(128),
    @PasswordSalt     nvarchar(128),
    @CurrentTimeUtc   datetime,
    @PasswordFormat   int = 0
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)
        
   ---- Modified to add Password History     
    DECLARE @cnt INT,
		@DateChanged DATETIME,
		@OldPassword NVARCHAR(128),
		@OldSalt NVARCHAR(128)
		
	SELECT @OldPassword = [Password], @OldSalt = PasswordSalt FROM dbo.aspnet_Membership WHERE UserID = @UserID	
    SELECT @cnt = COUNT(ID) FROM dbo.PasswordHistory WHERE USERID = @UserId
    SELECT @DateChanged = MIN(DateChanged) FROM dbo.PasswordHistory WHERE UserID = @UserId
    
    IF @Cnt = 4
		DELETE FROM dbo.PasswordHistory WHERE DateChanged = @DateChanged
	
	INSERT INTO dbo.PasswordHistory (UserID, [Password], PasswordSalt, DateChanged, UserName)
	VALUES (@UserId, @OldPassword,@OldSalt,GETDATE(), @UserName)
	
	----- End Password History Add  4/21/17

    UPDATE dbo.aspnet_Membership
    SET Password = @NewPassword, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt,
        LastPasswordChangedDate = @CurrentTimeUtc
    WHERE @UserId = UserId
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_ResetPassword]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_ResetPassword]
    @ApplicationName             nvarchar(256),
    @UserName                    nvarchar(256),
    @NewPassword                 nvarchar(128),
    @MaxInvalidPasswordAttempts  int,
    @PasswordAttemptWindow       int,
    @PasswordSalt                nvarchar(128),
    @CurrentTimeUtc              datetime,
    @PasswordFormat              int = 0,
    @PasswordAnswer              nvarchar(128) = NULL
AS
BEGIN
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @UserId                                 uniqueidentifier
    SET     @UserId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

   ---- Modified to add Password History     
    DECLARE @cnt INT,
		@DateChanged DATETIME,
		@OldPassword NVARCHAR(128),
		@OldSalt NVARCHAR(128)
		
	SELECT @OldPassword = [Password], @OldSalt = PasswordSalt FROM dbo.aspnet_Membership WHERE UserID = @UserID	
    SELECT @cnt = COUNT(ID) FROM dbo.PasswordHistory WHERE USERID = @UserId
    SELECT @DateChanged = MIN(DateChanged) FROM dbo.PasswordHistory WHERE UserID = @UserId
    
    IF @Cnt = 4
		DELETE FROM dbo.PasswordHistory WHERE DateChanged = @DateChanged
	
	INSERT INTO dbo.PasswordHistory (UserID, [Password], PasswordSalt, DateChanged, UserName)
	VALUES (@UserId, @OldPassword,@OldSalt,GETDATE(), @UserName)
	
	----- End Password History Add  4/21/17

    SELECT @IsLockedOut = IsLockedOut,
           @LastLockoutDate = LastLockoutDate,
           @FailedPasswordAttemptCount = FailedPasswordAttemptCount,
           @FailedPasswordAttemptWindowStart = FailedPasswordAttemptWindowStart,
           @FailedPasswordAnswerAttemptCount = FailedPasswordAnswerAttemptCount,
           @FailedPasswordAnswerAttemptWindowStart = FailedPasswordAnswerAttemptWindowStart
    FROM dbo.aspnet_Membership WITH ( UPDLOCK )
    WHERE @UserId = UserId

    IF( @IsLockedOut = 1 )
    BEGIN
        SET @ErrorCode = 99
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Membership
    SET    Password = @NewPassword,
           LastPasswordChangedDate = @CurrentTimeUtc,
           PasswordFormat = @PasswordFormat,
           PasswordSalt = @PasswordSalt
    WHERE  @UserId = UserId AND
           ( ( @PasswordAnswer IS NULL ) OR ( LOWER( PasswordAnswer ) = LOWER( @PasswordAnswer ) ) )

    IF ( @@ROWCOUNT = 0 )
        BEGIN
            IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAnswerAttemptWindowStart ) )
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = 1
            END
            ELSE
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount + 1
            END

            BEGIN
                IF( @FailedPasswordAnswerAttemptCount >= @MaxInvalidPasswordAttempts )
                BEGIN
                    SET @IsLockedOut = 1
                    SET @LastLockoutDate = @CurrentTimeUtc
                END
            END

            SET @ErrorCode = 3
        END
    ELSE
        BEGIN
            IF( @FailedPasswordAnswerAttemptCount > 0 )
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = 0
                SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            END
        END

    IF( NOT ( @PasswordAnswer IS NULL ) )
    BEGIN
        UPDATE dbo.aspnet_Membership
        SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
            FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
            FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
            FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
            FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
        WHERE @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByUserId]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByUserId]
    @UserId               uniqueidentifier,
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    IF ( @UpdateLastActivity = 1 )
    BEGIN
        UPDATE   dbo.aspnet_Users
        SET      LastActivityDate = @CurrentTimeUtc
        FROM     dbo.aspnet_Users
        WHERE    @UserId = UserId

        IF ( @@ROWCOUNT = 0 ) -- User ID not found
            RETURN -1
    END

    SELECT  m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate, m.LastLoginDate, u.LastActivityDate,
            m.LastPasswordChangedDate, u.UserName, m.IsLockedOut,
            m.LastLockoutDate
    FROM    dbo.aspnet_Users u, dbo.aspnet_Membership m
    WHERE   @UserId = u.UserId AND u.UserId = m.UserId

    IF ( @@ROWCOUNT = 0 ) -- User ID not found
       RETURN -1

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByName]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByName]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    DECLARE @UserId uniqueidentifier

    IF (@UpdateLastActivity = 1)
    BEGIN
        -- select user ID from aspnet_users table
        SELECT TOP 1 @UserId = u.UserId
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1

        UPDATE   dbo.aspnet_Users
        SET      LastActivityDate = @CurrentTimeUtc
        WHERE    @UserId = UserId

        SELECT m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut, m.LastLockoutDate
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE  @UserId = u.UserId AND u.UserId = m.UserId 
    END
    ELSE
    BEGIN
        SELECT TOP 1 m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut,m.LastLockoutDate
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1
    END

    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetUserByEmail]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByEmail]
    @ApplicationName  nvarchar(256),
    @Email            nvarchar(256)
AS
BEGIN
    IF( @Email IS NULL )
        SELECT  u.UserName
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                u.UserId = m.UserId AND
                m.LoweredEmail IS NULL
    ELSE
        SELECT  u.UserName
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                u.UserId = m.UserId AND
                LOWER(@Email) = m.LoweredEmail

    IF (@@rowcount = 0)
        RETURN(1)
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetPasswordWithFormat]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetPasswordWithFormat]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @UpdateLastLoginActivityDate    bit,
    @CurrentTimeUtc                 datetime
AS
BEGIN
    DECLARE @IsLockedOut                        bit
    DECLARE @UserId                             uniqueidentifier
    DECLARE @Password                           nvarchar(128)
    DECLARE @PasswordSalt                       nvarchar(128)
    DECLARE @PasswordFormat                     int
    DECLARE @FailedPasswordAttemptCount         int
    DECLARE @FailedPasswordAnswerAttemptCount   int
    DECLARE @IsApproved                         bit
    DECLARE @LastActivityDate                   datetime
    DECLARE @LastLoginDate                      datetime

    SELECT  @UserId          = NULL

    SELECT  @UserId = u.UserId, @IsLockedOut = m.IsLockedOut, @Password=Password, @PasswordFormat=PasswordFormat,
            @PasswordSalt=PasswordSalt, @FailedPasswordAttemptCount=FailedPasswordAttemptCount,
		    @FailedPasswordAnswerAttemptCount=FailedPasswordAnswerAttemptCount, @IsApproved=IsApproved,
            @LastActivityDate = LastActivityDate, @LastLoginDate = LastLoginDate
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF (@UserId IS NULL)
        RETURN 1

    IF (@IsLockedOut = 1)
        RETURN 99

    SELECT   @Password, @PasswordFormat, @PasswordSalt, @FailedPasswordAttemptCount,
             @FailedPasswordAnswerAttemptCount, @IsApproved, @LastLoginDate, @LastActivityDate

    IF (@UpdateLastLoginActivityDate = 1 AND @IsApproved = 1)
    BEGIN
        UPDATE  dbo.aspnet_Membership
        SET     LastLoginDate = @CurrentTimeUtc
        WHERE   UserId = @UserId

        UPDATE  dbo.aspnet_Users
        SET     LastActivityDate = @CurrentTimeUtc
        WHERE   @UserId = UserId
    END


    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetPassword]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetPassword]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @MaxInvalidPasswordAttempts     int,
    @PasswordAttemptWindow          int,
    @CurrentTimeUtc                 datetime,
    @PasswordAnswer                 nvarchar(128) = NULL
AS
BEGIN
    DECLARE @UserId                                 uniqueidentifier
    DECLARE @PasswordFormat                         int
    DECLARE @Password                               nvarchar(128)
    DECLARE @passAns                                nvarchar(128)
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId,
            @Password = m.Password,
            @passAns = m.PasswordAnswer,
            @PasswordFormat = m.PasswordFormat,
            @IsLockedOut = m.IsLockedOut,
            @LastLockoutDate = m.LastLockoutDate,
            @FailedPasswordAttemptCount = m.FailedPasswordAttemptCount,
            @FailedPasswordAttemptWindowStart = m.FailedPasswordAttemptWindowStart,
            @FailedPasswordAnswerAttemptCount = m.FailedPasswordAnswerAttemptCount,
            @FailedPasswordAnswerAttemptWindowStart = m.FailedPasswordAnswerAttemptWindowStart
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m WITH ( UPDLOCK )
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF ( @@rowcount = 0 )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    IF( @IsLockedOut = 1 )
    BEGIN
        SET @ErrorCode = 99
        GOTO Cleanup
    END

    IF ( NOT( @PasswordAnswer IS NULL ) )
    BEGIN
        IF( ( @passAns IS NULL ) OR ( LOWER( @passAns ) <> LOWER( @PasswordAnswer ) ) )
        BEGIN
            IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAnswerAttemptWindowStart ) )
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = 1
            END
            ELSE
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount + 1
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
            END

            BEGIN
                IF( @FailedPasswordAnswerAttemptCount >= @MaxInvalidPasswordAttempts )
                BEGIN
                    SET @IsLockedOut = 1
                    SET @LastLockoutDate = @CurrentTimeUtc
                END
            END

            SET @ErrorCode = 3
        END
        ELSE
        BEGIN
            IF( @FailedPasswordAnswerAttemptCount > 0 )
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = 0
                SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            END
        END

        UPDATE dbo.aspnet_Membership
        SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
            FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
            FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
            FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
            FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
        WHERE @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    IF( @ErrorCode = 0 )
        SELECT @Password, @PasswordFormat

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetNumberOfUsersOnline]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetNumberOfUsersOnline]
    @ApplicationName            nvarchar(256),
    @MinutesSinceLastInActive   int,
    @CurrentTimeUtc             datetime
AS
BEGIN
    DECLARE @DateActive datetime
    SELECT  @DateActive = DATEADD(minute,  -(@MinutesSinceLastInActive), @CurrentTimeUtc)

    DECLARE @NumOnline int
    SELECT  @NumOnline = COUNT(*)
    FROM    dbo.aspnet_Users u(NOLOCK),
            dbo.aspnet_Applications a(NOLOCK),
            dbo.aspnet_Membership m(NOLOCK)
    WHERE   u.ApplicationId = a.ApplicationId                  AND
            LastActivityDate > @DateActive                     AND
            a.LoweredApplicationName = LOWER(@ApplicationName) AND
            u.UserId = m.UserId
    RETURN(@NumOnline)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_GetAllUsers]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetAllUsers]
    @ApplicationName       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0


    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
    SELECT u.UserId
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u
    WHERE  u.ApplicationId = @ApplicationId AND u.UserId = m.UserId
    ORDER BY u.UserName

    SELECT @TotalRecords = @@ROWCOUNT

    SELECT u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY u.UserName
    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_FindUsersByName]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_FindUsersByName]
    @ApplicationName       nvarchar(256),
    @UserNameToMatch       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT u.UserId
        FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND u.LoweredUserName LIKE LOWER(@UserNameToMatch)
        ORDER BY u.UserName


    SELECT  u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY u.UserName

    SELECT  @TotalRecords = COUNT(*)
    FROM    #PageIndexForUsers
    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_FindUsersByEmail]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_FindUsersByEmail]
    @ApplicationName       nvarchar(256),
    @EmailToMatch          nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    IF( @EmailToMatch IS NULL )
        INSERT INTO #PageIndexForUsers (UserId)
            SELECT u.UserId
            FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
            WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND m.Email IS NULL
            ORDER BY m.LoweredEmail
    ELSE
        INSERT INTO #PageIndexForUsers (UserId)
            SELECT u.UserId
            FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
            WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND m.LoweredEmail LIKE LOWER(@EmailToMatch)
            ORDER BY m.LoweredEmail

    SELECT  u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY m.LoweredEmail

    SELECT  @TotalRecords = COUNT(*)
    FROM    #PageIndexForUsers
    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_CreateUser]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_CreateUser]
    @ApplicationName                        nvarchar(256),
    @UserName                               nvarchar(256),
    @Password                               nvarchar(128),
    @PasswordSalt                           nvarchar(128),
    @Email                                  nvarchar(256),
    @PasswordQuestion                       nvarchar(256),
    @PasswordAnswer                         nvarchar(128),
    @IsApproved                             bit,
    @CurrentTimeUtc                         datetime,
    @CreateDate                             datetime = NULL,
    @UniqueEmail                            int      = 0,
    @PasswordFormat                         int      = 0,
    @UserId                                 uniqueidentifier OUTPUT
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @NewUserId uniqueidentifier
    SELECT @NewUserId = NULL

    DECLARE @IsLockedOut bit
    SET @IsLockedOut = 0

    DECLARE @LastLockoutDate  datetime
    SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )

    DECLARE @FailedPasswordAttemptCount int
    SET @FailedPasswordAttemptCount = 0

    DECLARE @FailedPasswordAttemptWindowStart  datetime
    SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

    DECLARE @FailedPasswordAnswerAttemptCount int
    SET @FailedPasswordAnswerAttemptCount = 0

    DECLARE @FailedPasswordAnswerAttemptWindowStart  datetime
    SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

    DECLARE @NewUserCreated bit
    DECLARE @ReturnValue   int
    SET @ReturnValue = 0

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    SET @CreateDate = @CurrentTimeUtc

    SELECT  @NewUserId = UserId FROM dbo.aspnet_Users WHERE LOWER(@UserName) = LoweredUserName AND @ApplicationId = ApplicationId
    IF ( @NewUserId IS NULL )
    BEGIN
        SET @NewUserId = @UserId
        EXEC @ReturnValue = dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CreateDate, @NewUserId OUTPUT
        SET @NewUserCreated = 1
    END
    ELSE
    BEGIN
        SET @NewUserCreated = 0
        IF( @NewUserId <> @UserId AND @UserId IS NOT NULL )
        BEGIN
            SET @ErrorCode = 6
            GOTO Cleanup
        END
    END

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @ReturnValue = -1 )
    BEGIN
        SET @ErrorCode = 10
        GOTO Cleanup
    END

    IF ( EXISTS ( SELECT UserId
                  FROM   dbo.aspnet_Membership
                  WHERE  @NewUserId = UserId ) )
    BEGIN
        SET @ErrorCode = 6
        GOTO Cleanup
    END

    SET @UserId = @NewUserId

    IF (@UniqueEmail = 1)
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership m WITH ( UPDLOCK, HOLDLOCK )
                    WHERE ApplicationId = @ApplicationId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            SET @ErrorCode = 7
            GOTO Cleanup
        END
    END

    IF (@NewUserCreated = 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate = @CreateDate
        WHERE  @UserId = UserId
        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    INSERT INTO dbo.aspnet_Membership
                ( ApplicationId,
                  UserId,
                  Password,
                  PasswordSalt,
                  Email,
                  LoweredEmail,
                  PasswordQuestion,
                  PasswordAnswer,
                  PasswordFormat,
                  IsApproved,
                  IsLockedOut,
                  CreateDate,
                  LastLoginDate,
                  LastPasswordChangedDate,
                  LastLockoutDate,
                  FailedPasswordAttemptCount,
                  FailedPasswordAttemptWindowStart,
                  FailedPasswordAnswerAttemptCount,
                  FailedPasswordAnswerAttemptWindowStart )
         VALUES ( @ApplicationId,
                  @UserId,
                  @Password,
                  @PasswordSalt,
                  @Email,
                  LOWER(@Email),
                  @PasswordQuestion,
                  @PasswordAnswer,
                  @PasswordFormat,
                  @IsApproved,
                  @IsLockedOut,
                  @CreateDate,
                  @CreateDate,
                  @CreateDate,
                  @LastLockoutDate,
                  @FailedPasswordAttemptCount,
                  @FailedPasswordAttemptWindowStart,
                  @FailedPasswordAnswerAttemptCount,
                  @FailedPasswordAnswerAttemptWindowStart )

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer]
    @ApplicationName       nvarchar(256),
    @UserName              nvarchar(256),
    @NewPasswordQuestion   nvarchar(256),
    @NewPasswordAnswer     nvarchar(128)
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Membership m, dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId
    IF (@UserId IS NULL)
    BEGIN
        RETURN(1)
    END

    UPDATE dbo.aspnet_Membership
    SET    PasswordQuestion = @NewPasswordQuestion, PasswordAnswer = @NewPasswordAnswer
    WHERE  UserId=@UserId
    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_AnyDataInTables]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_AnyDataInTables]
    @TablesToCheck int
AS
BEGIN
    -- Check Membership table if (@TablesToCheck & 1) is set
    IF ((@TablesToCheck & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Membership))
        BEGIN
            SELECT N'aspnet_Membership'
            RETURN
        END
    END

    -- Check aspnet_Roles table if (@TablesToCheck & 2) is set
    IF ((@TablesToCheck & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Roles') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 RoleId FROM dbo.aspnet_Roles))
        BEGIN
            SELECT N'aspnet_Roles'
            RETURN
        END
    END

    -- Check aspnet_Profile table if (@TablesToCheck & 4) is set
    IF ((@TablesToCheck & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Profile))
        BEGIN
            SELECT N'aspnet_Profile'
            RETURN
        END
    END

    -- Check aspnet_PersonalizationPerUser table if (@TablesToCheck & 8) is set
    IF ((@TablesToCheck & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_PersonalizationPerUser))
        BEGIN
            SELECT N'aspnet_PersonalizationPerUser'
            RETURN
        END
    END

    -- Check aspnet_PersonalizationPerUser table if (@TablesToCheck & 16) is set
    IF ((@TablesToCheck & 16) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_WebEvent_LogEvent') AND (type = 'P'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 * FROM dbo.aspnet_WebEvent_Events))
        BEGIN
            SELECT N'aspnet_WebEvent_Events'
            RETURN
        END
    END

    -- Check aspnet_Users table if (@TablesToCheck & 1,2,4 & 8) are all set
    IF ((@TablesToCheck & 1) <> 0 AND
        (@TablesToCheck & 2) <> 0 AND
        (@TablesToCheck & 4) <> 0 AND
        (@TablesToCheck & 8) <> 0 AND
        (@TablesToCheck & 32) <> 0 AND
        (@TablesToCheck & 128) <> 0 AND
        (@TablesToCheck & 256) <> 0 AND
        (@TablesToCheck & 512) <> 0 AND
        (@TablesToCheck & 1024) <> 0)
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Users))
        BEGIN
            SELECT N'aspnet_Users'
            RETURN
        END
        IF (EXISTS(SELECT TOP 1 ApplicationId FROM dbo.aspnet_Applications))
        BEGIN
            SELECT N'aspnet_Applications'
            RETURN
        END
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @Path             NVARCHAR(256),
    @PageSettings     IMAGE,
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Paths_CreatePath @ApplicationId, @Path, @PathId OUTPUT
    END

    IF (EXISTS(SELECT PathId FROM dbo.aspnet_PersonalizationAllUsers WHERE PathId = @PathId))
        UPDATE dbo.aspnet_PersonalizationAllUsers SET PageSettings = @PageSettings, LastUpdatedDate = @CurrentTimeUtc WHERE PathId = @PathId
    ELSE
        INSERT INTO dbo.aspnet_PersonalizationAllUsers(PathId, PageSettings, LastUpdatedDate) VALUES (@PathId, @PageSettings, @CurrentTimeUtc)
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @Path              NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    DELETE FROM dbo.aspnet_PersonalizationAllUsers WHERE PathId = @PathId
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @Path              NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT p.PageSettings FROM dbo.aspnet_PersonalizationAllUsers p WHERE p.PathId = @PathId
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_ResetUserState]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_ResetUserState] (
    @Count                  int                 OUT,
    @ApplicationName        NVARCHAR(256),
    @InactiveSinceDate      DATETIME            = NULL,
    @UserName               NVARCHAR(256)       = NULL,
    @Path                   NVARCHAR(256)       = NULL)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser
        WHERE Id IN (SELECT PerUser.Id
                     FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
                     WHERE Paths.ApplicationId = @ApplicationId
                           AND PerUser.UserId = Users.UserId
                           AND PerUser.PathId = Paths.PathId
                           AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
                           AND (@UserName IS NULL OR Users.LoweredUserName = LOWER(@UserName))
                           AND (@Path IS NULL OR Paths.LoweredPath = LOWER(@Path)))

        SELECT @Count = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_ResetSharedState]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_ResetSharedState] (
    @Count int OUT,
    @ApplicationName NVARCHAR(256),
    @Path NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationAllUsers
        WHERE PathId IN
            (SELECT AllUsers.PathId
             FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
             WHERE Paths.ApplicationId = @ApplicationId
                   AND AllUsers.PathId = Paths.PathId
                   AND Paths.LoweredPath = LOWER(@Path))

        SELECT @Count = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_GetCountOfState]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_GetCountOfState] (
    @Count int OUT,
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @Path NVARCHAR(256) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @InactiveSinceDate DATETIME = NULL)
AS
BEGIN

    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
        IF (@AllUsersScope = 1)
            SELECT @Count = COUNT(*)
            FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
            WHERE Paths.ApplicationId = @ApplicationId
                  AND AllUsers.PathId = Paths.PathId
                  AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
        ELSE
            SELECT @Count = COUNT(*)
            FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
            WHERE Paths.ApplicationId = @ApplicationId
                  AND PerUser.UserId = Users.UserId
                  AND PerUser.PathId = Paths.PathId
                  AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
                  AND (@UserName IS NULL OR Users.LoweredUserName LIKE LOWER(@UserName))
                  AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_FindState]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_FindState] (
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @PageIndex              INT,
    @PageSize               INT,
    @Path NVARCHAR(256) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @InactiveSinceDate DATETIME = NULL)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        RETURN

    -- Set the page bounds
    DECLARE @PageLowerBound INT
    DECLARE @PageUpperBound INT
    DECLARE @TotalRecords   INT
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table to store the selected results
    CREATE TABLE #PageIndex (
        IndexId int IDENTITY (0, 1) NOT NULL,
        ItemId UNIQUEIDENTIFIER
    )

    IF (@AllUsersScope = 1)
    BEGIN
        -- Insert into our temp table
        INSERT INTO #PageIndex (ItemId)
        SELECT Paths.PathId
        FROM dbo.aspnet_Paths Paths,
             ((SELECT Paths.PathId
               FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
               WHERE Paths.ApplicationId = @ApplicationId
                      AND AllUsers.PathId = Paths.PathId
                      AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              ) AS SharedDataPerPath
              FULL OUTER JOIN
              (SELECT DISTINCT Paths.PathId
               FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Paths Paths
               WHERE Paths.ApplicationId = @ApplicationId
                      AND PerUser.PathId = Paths.PathId
                      AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              ) AS UserDataPerPath
              ON SharedDataPerPath.PathId = UserDataPerPath.PathId
             )
        WHERE Paths.PathId = SharedDataPerPath.PathId OR Paths.PathId = UserDataPerPath.PathId
        ORDER BY Paths.Path ASC

        SELECT @TotalRecords = @@ROWCOUNT

        SELECT Paths.Path,
               SharedDataPerPath.LastUpdatedDate,
               SharedDataPerPath.SharedDataLength,
               UserDataPerPath.UserDataLength,
               UserDataPerPath.UserCount
        FROM dbo.aspnet_Paths Paths,
             ((SELECT PageIndex.ItemId AS PathId,
                      AllUsers.LastUpdatedDate AS LastUpdatedDate,
                      DATALENGTH(AllUsers.PageSettings) AS SharedDataLength
               FROM dbo.aspnet_PersonalizationAllUsers AllUsers, #PageIndex PageIndex
               WHERE AllUsers.PathId = PageIndex.ItemId
                     AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
              ) AS SharedDataPerPath
              FULL OUTER JOIN
              (SELECT PageIndex.ItemId AS PathId,
                      SUM(DATALENGTH(PerUser.PageSettings)) AS UserDataLength,
                      COUNT(*) AS UserCount
               FROM aspnet_PersonalizationPerUser PerUser, #PageIndex PageIndex
               WHERE PerUser.PathId = PageIndex.ItemId
                     AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
               GROUP BY PageIndex.ItemId
              ) AS UserDataPerPath
              ON SharedDataPerPath.PathId = UserDataPerPath.PathId
             )
        WHERE Paths.PathId = SharedDataPerPath.PathId OR Paths.PathId = UserDataPerPath.PathId
        ORDER BY Paths.Path ASC
    END
    ELSE
    BEGIN
        -- Insert into our temp table
        INSERT INTO #PageIndex (ItemId)
        SELECT PerUser.Id
        FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
        WHERE Paths.ApplicationId = @ApplicationId
              AND PerUser.UserId = Users.UserId
              AND PerUser.PathId = Paths.PathId
              AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              AND (@UserName IS NULL OR Users.LoweredUserName LIKE LOWER(@UserName))
              AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
        ORDER BY Paths.Path ASC, Users.UserName ASC

        SELECT @TotalRecords = @@ROWCOUNT

        SELECT Paths.Path, PerUser.LastUpdatedDate, DATALENGTH(PerUser.PageSettings), Users.UserName, Users.LastActivityDate
        FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths, #PageIndex PageIndex
        WHERE PerUser.Id = PageIndex.ItemId
              AND PerUser.UserId = Users.UserId
              AND PerUser.PathId = Paths.PathId
              AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
        ORDER BY Paths.Path ASC, Users.UserName ASC
    END

    RETURN @TotalRecords
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationAdministration_DeleteAllState]    Script Date: 10/03/2019 13:59:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_DeleteAllState] (
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @Count int OUT)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        IF (@AllUsersScope = 1)
            DELETE FROM aspnet_PersonalizationAllUsers
            WHERE PathId IN
               (SELECT Paths.PathId
                FROM dbo.aspnet_Paths Paths
                WHERE Paths.ApplicationId = @ApplicationId)
        ELSE
            DELETE FROM aspnet_PersonalizationPerUser
            WHERE PathId IN
               (SELECT Paths.PathId
                FROM dbo.aspnet_Paths Paths
                WHERE Paths.ApplicationId = @ApplicationId)

        SELECT @Count = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Roles_DeleteRole]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Roles_DeleteRole]
    @ApplicationName            nvarchar(256),
    @RoleName                   nvarchar(256),
    @DeleteOnlyIfRoleIsEmpty    bit
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    DECLARE @RoleId   uniqueidentifier
    SELECT  @RoleId = NULL
    SELECT  @RoleId = RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
    BEGIN
        SELECT @ErrorCode = 1
        GOTO Cleanup
    END
    IF (@DeleteOnlyIfRoleIsEmpty <> 0)
    BEGIN
        IF (EXISTS (SELECT RoleId FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId))
        BEGIN
            SELECT @ErrorCode = 2
            GOTO Cleanup
        END
    END


    DELETE FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DELETE FROM dbo.aspnet_Roles WHERE @RoleId = RoleId  AND ApplicationId = @ApplicationId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_SetPageSettings]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_SetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @PageSettings     IMAGE,
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Paths_CreatePath @ApplicationId, @Path, @PathId OUTPUT
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CurrentTimeUtc, @UserId OUTPUT
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    IF (EXISTS(SELECT PathId FROM dbo.aspnet_PersonalizationPerUser WHERE UserId = @UserId AND PathId = @PathId))
        UPDATE dbo.aspnet_PersonalizationPerUser SET PageSettings = @PageSettings, LastUpdatedDate = @CurrentTimeUtc WHERE UserId = @UserId AND PathId = @PathId
    ELSE
        INSERT INTO dbo.aspnet_PersonalizationPerUser(UserId, PathId, PageSettings, LastUpdatedDate) VALUES (@UserId, @PathId, @PageSettings, @CurrentTimeUtc)
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        RETURN
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE PathId = @PathId AND UserId = @UserId
    RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_PersonalizationPerUser_GetPageSettings]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_GetPageSettings] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        RETURN
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    SELECT p.PageSettings FROM dbo.aspnet_PersonalizationPerUser p WHERE p.PathId = @PathId AND p.UserId = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[GetPasswordSalt]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 5/19/2017
-- Description:	Returns a Password Salt for a specific user
-- =============================================
CREATE PROCEDURE [dbo].[GetPasswordSalt]
	-- Add the parameters for the stored procedure here
	@lowerusername varchar(256) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		SELECT PasswordSalt FROM dbo.aspnet_Membership am
		INNER JOIN dbo.aspnet_Users au ON am.UserId = au.UserId
		WHERE au.LoweredUserName = @lowerusername
END
GO
/****** Object:  StoredProcedure [dbo].[GetPasswordHistoryByName]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 21 April 2017
-- Description:	This stored procedure retrieves the
--		password history for an individual user by username
-- =============================================
CREATE PROCEDURE [dbo].[GetPasswordHistoryByName]
	-- Add the parameters for the stored procedure here
	@UserName NVARCHAR(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Password], PasswordSalt, DateChanged FROM dbo.PasswordHistory 
	WHERE UserName = @UserName
	UNION
	SELECT [Password], PasswordSalt, LastPasswordChangedDate AS DateChanged
	FROM dbo.aspnet_Membership am INNER JOIN dbo.aspnet_Users au 
		ON au.UserID = am.UserID
	WHERE  au.UserName = @UserName
END
GO
/****** Object:  StoredProcedure [dbo].[GetPasswordHistoryByID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 21 April 2017
-- Description:	This stored procedure retrieves the
--		password history for an individual user
-- =============================================
CREATE PROCEDURE [dbo].[GetPasswordHistoryByID]
	-- Add the parameters for the stored procedure here
	@UserID UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Password], PasswordSalt, DateChanged FROM dbo.PasswordHistory 
	WHERE UserID = @UserID
	UNION
	SELECT [Password], PasswordSalt, LastPasswordChangedDate AS DateChanged
	FROM dbo.aspnet_Membership am 
	WHERE  am.UserID = @UserID
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndivSiteCodes]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 11/8/2016
-- Description:	Get Site IDs from union table for a
--		particular individual
-- =============================================
CREATE PROCEDURE [dbo].[GetIndivSiteCodes]
	-- Add the parameters for the stored procedure here
	@IndivID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT SiteCode 
	FROM dbo.IndividualIDSiteCodeUnion
	WHERE IndividualID = @IndivID
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualTypesByIndividualID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 7 Nov 2014
-- Description:	Return all individual type records associated with the provided individual id.
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualTypesByIndividualID]
	-- Add the parameters for the stored procedure here
	@IndividualID	INT--,
	--@IsActive bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
	t.ID,
	t.IndividualID,
	t.IsActive,
	t.TypeID,
	lt.Text AS TypeDescription,
	t.ModifiedBy,
	t.DateLastModified
	FROM dbo.IndividualType t
	INNER JOIN dbo.LookupTable lt ON t.TypeID = lt.ID
	WHERE t.IndividualID = @IndividualID
	--AND t.IsActive = @IsActive
	AND lt.Code = 'IndividualType'
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualsBySiteCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualsBySiteCode] 
@SiteCode		VARCHAR(6),
@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	----- AUDIT MultiRead - 10/17/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @PatientList VarChar(Max)
	Declare @Notes VarChar (Max)
		
	Select @PatientList = Coalesce (@PatientList,'') + CAST(ID as VarChar(20)) + ', ' 
	FROM dbo.Individual 
	WHERE SiteCodeID = @SiteCode

	Set @Notes = 'Individual Records of Personnel at a site.  SiteCode = ' + @siteCode
	
	If @PatientList is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 5, @PatientList, @Notes
	
	-----------------------

	SELECT Comments, 
	i.DateLastModified, 
	DateOfBirth, 
	Demographic, 
	EADStopDate, 
	FirstName, 
	i.ID, 
	i.IsActive, 
	IsPOC, 
	LastName, 
	TheaterLocationCode, 
	MiddleName, 
	i.ModifiedBy, 
	lt.Value as PersonalType, 
	SiteCodeID
	FROM dbo.Individual i
	INNER JOIN dbo.IndividualType it ON i.ID = it.IndividualID
	INNER JOIN dbo.LookupTable lt ON lt.ID = it.TypeID
	WHERE SiteCodeID = @SiteCode

END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualsAtSite]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 16 May 14
-- Description:	This stored procedure returns all of the individuals at a site based on site code.
-- Modified:   3/24/15 - kdb - to remove duplicate personnel from list
--				10/20/15 - kdb - to add individual site code union table for site verification
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualsAtSite]
	@siteCode varchar(6),
	@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	----- Audit MultiRead - 10/17/14 - BAF
	Declare @ReadDate datetime = GetDate()
	Declare @PatientList varchar(Max) 
	Declare @Notes VarChar(Max)
	
	Select @PatientList = Coalesce (@PatientList,'') + CAST(i.ID as VarChar(20)) + ', ' 
	FROM dbo.Individual i
	INNER JOIN dbo.IndividualType it ON i.ID = it.IndividualID
	INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.ID
	WHERE i.SiteCodeID = @SiteCode
		AND lower(lt.Value) <> 'patient';

	Set @Notes = 'Individual Records of Personnel at a site other than patients.  SiteCode = ' + @siteCode;
	If @PatientList is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 5, @PatientList, @Notes;
	-------------------------------

	select a.ID, a.LastName, a.FirstName, a.Comments, a.DateLastModified, a.DateOfBirth, a.Demographic, a.EADStopDate, 
	a.IsActive, a.IsPOC, a.LegacyID, a.MiddleName, a.ModifiedBy, a.PersonalType, a.SiteCodeID, a.TheaterLocationCode, 
	a.IDNumber, a.IDNumberType
	From
	(
	SELECT Distinct
	i.ID,i.LastName,i.FirstName,i.Comments, i.DateLastModified,i.DateOfBirth,i.Demographic,i.EADStopDate,i.IsActive,
	i.IsPOC,i.LegacyID,i.MiddleName,i.ModifiedBy,lt.Value AS PersonalType,i.SiteCodeID,i.TheaterLocationCode,
	'' AS IDNumber,'' AS IDNumberType,
	ROW_NUMBER() over (partition by i.ID order by i.ID) as RowID
	FROM dbo.Individual i
	INNER JOIN dbo.IndividualIDSiteCodeUnion ui on i.ID = ui.IndividualID
	INNER JOIN dbo.IndividualType it ON it.IndividualID = i.ID
	INNER JOIN dbo.LookupTable lt ON lt.ID = it.TypeID
	WHERE (i.SiteCodeID = @siteCode OR ui.SiteCode = @siteCode)
	AND lower(lt.Value) <> 'patient'
	) a
	where a.PersonalType <> 'PATIENT' and
		a.RowID = 1 and
		 a.IsActive = 1
	Order by LastName, FirstName
 END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualPhoneNumbersByIndividualID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualPhoneNumbersByIndividualID] 
@IndividualID	INT,
@IsActive		bit = 1,
@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	---- Audit Multi Read Routine - 10/15/14 - BAF
	Declare @ReadDate	DateTime = GetDate()
	Declare @Patient	varchar(200) = Cast(@IndividualID as VarChar(200))
	Declare @Notes		VarChar(Max)
	
	Select @Notes = Coalesce (@Notes,'Phone Record IDs: ') + CAST(ID as VarChar(20)) + ', '
	from dbo.IndividualPhoneNumber 
	where IndividualID = @IndividualID
	
	If @Notes is not Null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 7, @Patient, @Notes
	---------------------------

	IF(@IsActive = 1)
	BEGIN
		SELECT AreaCode, 
		DateLastModified, 
		Extension, 
		ID, 
		IndividualID, 
		IsActive, 
		ModifiedBy, 
		AreaCode + PhoneNumber as PhoneNumber, 
		PhoneNumberType 
		FROM dbo.IndividualPhoneNumber
		WHERE IndividualID = @IndividualID
--		AND IsActive = 1
	END
	ELSE
	BEGIN
		SELECT AreaCode, 
		DateLastModified, 
		Extension, 
		ID, 
		IndividualID, 
		IsActive, 
		ModifiedBy, 
		AreaCode + PhoneNumber as PhoneNumber, 
		PhoneNumberType 
		FROM dbo.IndividualPhoneNumber
		WHERE IndividualID = @IndividualID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualIDNumbersByIndividualID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualIDNumbersByIndividualID] 
@IndividualID	INT,
@IsActive bit = 1,
@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	---- Audit Multi Read - 10/15/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @Patient varchar(200) = Cast(@IndividualID as VarChar(200))
	Declare @Notes VarChar(Max)
	
	Select @Notes = Coalesce (@Notes,'Individual ID Numbers: ') + CAST(ID as VarChar(20)) + ', ' 
	from dbo.IndividualIDNumber where IndividualID = @IndividualID
	
	Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 21, @Patient, @Notes
	------------------------

	IF(@IsActive = 1)
	BEGIN
		SELECT ID, 
		IndividualID, 
		IDNumber, 
		IDNumberType, 
		IsActive, 
		ModifiedBy, 
		DateLastModified 
		FROM dbo.IndividualIDNumber	
		WHERE IndividualID = @IndividualID
--		AND IsActive = 1
	END
	ELSE
	BEGIN
		SELECT ID, 
		IndividualID, 
		IDNumber, 
		IDNumberType, 
		IsActive, 
		ModifiedBy, 
		DateLastModified 
		FROM dbo.IndividualIDNumber	
		WHERE IndividualID = @IndividualID
	END
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualIDNumberByIDNumber]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualIDNumberByIDNumber] 
@IDNumber VARCHAR(15),
@IDNumberType VARCHAR(3),
@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	---- Audit SingleRead -- 10/17/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @PatientID int
	Declare @RecID int
	
	Select @PatientID = IndividualID from dbo.IndividualIDNumber 
		where IDNumber = @IDNumber and IDNumberType = @IDNumberType
		
	Select @RecID = ID from dbo.IndividualIDNumber where
		IDNumber = @IDNumber and IDNumberType = @IDNumberType
		
	If @PatientID is not Null
		Exec InsertAuditSingleRead @ReadDate, @PatientID, 21, @ModifiedBy, @RecID
	---------------------

	SELECT ID, 
	IndividualID, 
	IDNumber, 
	IDNumberType, 
	IsActive, 
	ModifiedBy, 
	DateLastModified 
	FROM dbo.IndividualIDNumber
	WHERE IDNumber = @IDNumber
	AND IDNumberType = @IDNumberType	
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualIDNumberByID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualIDNumberByID] 
@ID	INT,
@ModifiedBy VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	---- Audit Single Record - 10/17/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @PatientID int 
	Declare @RecID Varchar(50)
	
	Set @RecID = CAST(@ID as VarChar(50))
	
	Select @PatientID = IndividualID
	From dbo.IndividualIDNumber where ID = @ID
	
	Exec InsertAuditSingleRead @ReadDate, @PatientID, 21, @ModifiedBy, @RecID
	-------
	SELECT ID, 
	IndividualID, 
	IDNumber, 
	IDNumberType, 
	IsActive, 
	ModifiedBy, 
	DateLastModified 
	FROM dbo.IndividualIDNumber
	WHERE ID = @ID
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetFramesByEligibility]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  2/23/16 - baf - Added FrameType to select
--            1/30/17 - baf - Added check for CG and Frames A1000
-- =============================================
CREATE PROCEDURE [dbo].[GetFramesByEligibility] 
@Demographic	VARCHAR(8),
@SiteCode		VARCHAR(6),
@RxID			INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DemoChanged VARCHAR(7),
		@BOS Char(1), @Priority Char(1)
		
	SELECT @DemoChanged = SUBSTRING(@Demographic, 1,6) + 'B'
	Select @BOS = SUBSTRING(@Demographic,4,1), @Priority = RIGHT(@Demographic,1)
	Print @BOS
If @BOS = 'C'
	Begin
	SELECT DISTINCT x.* FROM
	(
			SELECT a.FrameCode,
			a.FrameDescription,
			a.FrameCode + ' - ' + a.FrameDescription as FrameLongDescription,
			a.FrameNotes,
			a.MaxPair,
			a.ImageURL,
			a.IsActive,
			a.ModifiedBy,
			a.DateLastModified,
			a.IsInsert,
			a.IsFOC,
			a.FrameType
			FROM  dbo.Frame a
			INNER JOIN dbo.FrameEligibilityFrameUnion b ON a.FrameCode = b.FrameCode 
			INNER JOIN dbo.FrameItemEligibilityFrameItemUnion c ON c.FrameCode = a.FrameCode
			WHERE b.EligibilityFrame = SUBSTRING(@Demographic, 1, 7) OR
			b.EligibilityFrame = @DemoChanged AND
			RIGHT(@Demographic,1) = RIGHT(c.EligibiityFrameItem,1)
			) x
	LEFT JOIN dbo.FrameRestrictions fr ON x.FrameCode = fr.FrameCode
	LEFT JOIN dbo.RxFrameRestrictions rf ON x.FrameCode = rf.FrameCode
	JOIN dbo.Prescription rx ON rx.ID = @RxID
	WHERE (@SiteCode = fr.SiteCode 
		--(SELECT DISTINCT SiteCode FROM dbo.FrameRestrictions WHERE SiteType = 'CLINIC' AND Framecode = x.FrameCode)
		OR fr.SiteCode = 'NONE')
		AND x.IsActive = 1
		AND (((x.FrameCode = rf.FrameCode) and ((rx.ODSphere <= rf.MaxSphere AND  rx.ODSphere >= rf.MinSphere) AND (rx.OSSphere <= rf.MaxSphere AND rx.OSSphere >= rf.MinSphere)	
			AND rx.ODCylinder >= rf.MinCylinder AND rx.OSCylinder >= rf.MinCylinder)
			OR (rf.FrameCode IS NULL)))
 		and (@Priority <> 'S' or (x.FrameCode not in ('AFF','AFJS') and @Priority = 'S'))
	ORDER BY x.FrameCode
	END
ELSE
	BEGIN
	If @BOS <> 'F'
		Begin
		SELECT DISTINCT x.* FROM
		(
			SELECT a.FrameCode,
			a.FrameDescription,
			a.FrameCode + ' - ' + a.FrameDescription as FrameLongDescription,
			a.FrameNotes,
			a.MaxPair,
			a.ImageURL,
			a.IsActive,
			a.ModifiedBy,
			a.DateLastModified,
			a.IsInsert,
			a.IsFOC,
			a.FrameType
			FROM  dbo.Frame a
			INNER JOIN dbo.FrameEligibilityFrameUnion b ON a.FrameCode = b.FrameCode 
			INNER JOIN dbo.FrameItemEligibilityFrameItemUnion c ON c.FrameCode = a.FrameCode
			WHERE b.EligibilityFrame = SUBSTRING(@Demographic, 1, 7) OR
			b.EligibilityFrame = @DemoChanged AND
			RIGHT(@Demographic,1) = RIGHT(c.EligibiityFrameItem,1)
			) x
		LEFT JOIN dbo.FrameRestrictions fr ON x.FrameCode = fr.FrameCode
		LEFT JOIN dbo.RxFrameRestrictions rf ON x.FrameCode = rf.FrameCode
		JOIN dbo.Prescription rx ON rx.ID = @RxID
		WHERE x.FrameCode <> 'A1000' and
			(@SiteCode = fr.SiteCode 
			--(SELECT DISTINCT SiteCode FROM dbo.FrameRestrictions WHERE SiteType = 'CLINIC' AND Framecode = x.FrameCode)
			OR fr.SiteCode = 'NONE')
			AND x.IsActive = 1
			AND (((x.FrameCode = rf.FrameCode) and ((rx.ODSphere <= rf.MaxSphere AND  rx.ODSphere >= rf.MinSphere) AND (rx.OSSphere <= rf.MaxSphere AND rx.OSSphere >= rf.MinSphere)	
				AND rx.ODCylinder >= rf.MinCylinder AND rx.OSCylinder >= rf.MinCylinder)
				OR (rf.FrameCode IS NULL)))
			and (@Priority <> 'S' or (x.FrameCode not in ('AFF','AFJS') and @Priority = 'S'))
		ORDER BY x.FrameCode
		END
	ELSE
		Begin
		SELECT DISTINCT x.* FROM
		(
			SELECT a.FrameCode,
			a.FrameDescription,
			a.FrameCode + ' - ' + a.FrameDescription as FrameLongDescription,
			a.FrameNotes,
			a.MaxPair,
			a.ImageURL,
			a.IsActive,
			a.ModifiedBy,
			a.DateLastModified,
			a.IsInsert,
			a.IsFOC,
			a.FrameType
			FROM  dbo.Frame a
			INNER JOIN dbo.FrameEligibilityFrameUnion b ON a.FrameCode = b.FrameCode 
			INNER JOIN dbo.FrameItemEligibilityFrameItemUnion c ON c.FrameCode = a.FrameCode
			WHERE b.EligibilityFrame = SUBSTRING(@Demographic, 1, 7) OR
			b.EligibilityFrame = @DemoChanged AND
			RIGHT(@Demographic,1) = RIGHT(c.EligibiityFrameItem,1)
			) x
		LEFT JOIN dbo.FrameRestrictions fr ON x.FrameCode = fr.FrameCode
		LEFT JOIN dbo.RxFrameRestrictions rf ON x.FrameCode = rf.FrameCode
		JOIN dbo.Prescription rx ON rx.ID = @RxID
		WHERE x.FrameCode <> 'A1000' and
			(@SiteCode = fr.SiteCode 
			--(SELECT DISTINCT SiteCode FROM dbo.FrameRestrictions WHERE SiteType = 'CLINIC' AND Framecode = x.FrameCode)
			OR fr.SiteCode = 'NONE')
			AND x.IsActive = 1
			AND (((x.FrameCode = rf.FrameCode) and ((rx.ODSphere <= rf.MaxSphere AND  rx.ODSphere >= rf.MinSphere) AND (rx.OSSphere <= rf.MaxSphere AND rx.OSSphere >= rf.MinSphere)	
				AND rx.ODCylinder >= rf.MinCylinder AND rx.OSCylinder >= rf.MinCylinder)
				OR (rf.FrameCode IS NULL)))
		ORDER BY x.FrameCode		
		End
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualEMailAddressesByIndividualID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualEMailAddressesByIndividualID] 
@IndividualID	INT,
@IsActive bit = 1,
@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	------ Audit Routine Multi Read - 10/15/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @Patient varchar(200) = Cast(@IndividualID as VarChar(200))
	Declare @Notes VarChar(Max) 
	
	Select @Notes = Coalesce (@Notes, 'EMAIL Address IDs: ') + CAST(ID as VarChar(20)) + ', '
	From dbo.IndividualEMailAddress where IndividualID = @IndividualID
	
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 9, @IndividualID, @Notes
	------------------------------

    -- Insert statements for procedure here
	IF(@IsActive = 1)
	BEGIN
		SELECT ID,
		IndividualID,
		EMailType, 
		EMailAddress,
		IsActive, 
		ModifiedBy,
		DateLastModified 
		FROM dbo.IndividualEMailAddress
		WHERE IndividualID = @IndividualID
		AND IsActive = 1
	END
	ELSE
	BEGIN
		SELECT ID,
		IndividualID,
		EMailType, 
		EMailAddress,
		IsActive, 
		ModifiedBy,
		DateLastModified 
		FROM dbo.IndividualEMailAddress
		WHERE IndividualID = @IndividualID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualBySiteCodeAndPersonalType]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualBySiteCodeAndPersonalType] 
@SiteCode		VARCHAR(6),
@PersonalType	VARCHAR(25),
@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TypeID  INT
	
	SELECT @TypeID = ID FROM dbo.LookupTable
	WHERE Value = @PersonalType AND Code = 'IndividualType'
	
	----- Auditing MultiRead 10/17/14 - BAF
	Declare @ReadDate Datetime = GetDate()
	Declare @PatientList as VarChar(Max)
	Declare @Notes as VarChar(Max)

	Set @Notes = 'Personal info is received for a specified PersonalType at a given site.  SiteCode = ' + IsNull(@SiteCode,'')
		+ ', PersonalType = ' + IsNull(@PersonalType,'')
		
	Select @PatientList = Coalesce (@PatientList,'') + CAST(a.ID as VarChar(20)) + ', '
	FROM dbo.Individual a
			INNER JOIN dbo.IndividualIDSiteCodeUnion b ON a.ID = b.IndividualID
			INNER JOIN dbo.IndividualType it ON it.IndividualID = a.ID
			WHERE b.SiteCode = @SiteCode 
				AND it.TypeID = @TypeID 
				and a.IsActive = 1
	
	If @PatientList is not NULL
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 5,@PatientList, @Notes
	--------------------

	IF(@SiteCode IS NOT NULL)
	BEGIN
		IF(LOWER(@PersonalType) = LOWER('TECHNICIAN') OR LOWER(@PersonalType) = LOWER('PROVIDER'))
		BEGIN
			SELECT DISTINCT
			a.Comments, 
			a.DateLastModified, 
			a.DateOfBirth, 
			a.Demographic, 
			a.EADStopDate, 
			a.FirstName, 
			a.ID, 
			a.IsActive, 
			a.IsPOC, 
			a.LastName, 
			a.TheaterLocationCode, 
			a.MiddleName, 
			a.ModifiedBy, 
			lt.Value AS PersonalType,
			b.SiteCode as 'SiteCodeID'
			FROM dbo.Individual a 
			INNER JOIN dbo.IndividualIDSiteCodeUnion b ON a.ID = b.IndividualID
			INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID and it.IsActive = 1
			INNER JOIN dbo.LookupTable lt ON lt.ID = it.TypeID
			WHERE b.SiteCode = @SiteCode AND
				it.TypeID = @TypeID and a.IsActive = 1
			ORDER BY a.LastName, a.FirstName
		END
		ELSE
		BEGIN
			SELECT DISTINCT
			Comments, 
			i.DateLastModified, 
			DateOfBirth, 
			Demographic, 
			EADStopDate, 
			FirstName, 
			i.ID, 
			i.IsActive, 
			IsPOC, 
			LastName, 
			TheaterLocationCode, 
			MiddleName, 
			i.ModifiedBy, 
			lt.Value AS PersonalType, 
			SiteCodeID
			FROM dbo.Individual i
			INNER JOIN dbo.IndividualType it ON i.id = it.IndividualID and it.IsActive = 1
			INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.id
			WHERE SiteCodeID = @SiteCode AND
				it.TypeID = @TypeID and i.IsActive = 1
			ORDER BY LastName, FirstName
		END
	END
	ELSE
	BEGIN
		IF(LOWER(@PersonalType) = LOWER('TECHNICIAN') OR LOWER(@PersonalType) = LOWER('PROVIDER'))
		BEGIN
			SELECT a.Comments, 
			a.DateLastModified, 
			a.DateOfBirth, 
			a.Demographic, 
			a.EADStopDate, 
			a.FirstName, 
			a.ID, 
			a.IsActive, 
			a.IsPOC, 
			a.LastName, 
			a.TheaterLocationCode, 
			a.MiddleName, 
			a.ModifiedBy, 
			lt.Value AS PersonalType, 
			b.SiteCode as 'SiteCodeID'
			FROM dbo.Individual a 
			INNER JOIN dbo.IndividualIDSiteCodeUnion b ON a.ID = b.IndividualID
			INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID and it.IsActive = 1
			INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.id
			WHERE it.TypeID = @TypeID and a.IsActive = 1
			ORDER BY a.LastName, a.FirstName
		END
		ELSE
		BEGIN
			SELECT Comments, 
			i.DateLastModified, 
			DateOfBirth, 
			Demographic, 
			EADStopDate, 
			FirstName, 
			i.ID, 
			i.IsActive, 
			IsPOC, 
			LastName, 
			TheaterLocationCode, 
			MiddleName, 
			i.ModifiedBy, 
			lt.Value AS PersonalType, 
			SiteCodeID
			FROM dbo.Individual i
			INNER JOIN dbo.IndividualType it ON i.id = it.IndividualID and it.IsActive = 1
			INNER JOIN dbo.LookupTable lt ON lt.ID = it.TypeID
			WHERE it.TypeID = @TypeID and i.IsActive = 1
			ORDER BY LastName, FirstName
		END
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualByPartialLastNameAndPersonalType]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- MODIFIED: 7 Nov 14 - To use IndividualType table for 
--		PersonalType Lookups - BAF
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualByPartialLastNameAndPersonalType] 
@LastName		varchar(75),
@PersonalType	varchar(25),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TempSearch varchar(51),
		@TypeID		INT
		
	SELECT @TempSearch = @LastName + '%'
	
	SELECT @TypeID = ID FROM dbo.LookupTable 
	WHERE Code = 'IndividualType' AND Value = @PersonalType
	
	------------------ Insert AuditSingleRead - 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE() 
	DECLARE @PatientID		INT
	DECLARE @ReadRecID		VARCHAR(50)
	
	SELECT @PatientID = a.ID 
	FROM dbo.Individual a
		INNER JOIN dbo.IndividualIDSiteCodeUnion b on a.ID = b.IndividualID
		INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID 
		INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.ID
	WHERE a.LastName LIKE @TempSearch AND 
		LOWER(lt.Value) = LOWER(@PersonalType)
		
		
	SET @ReadRecID = 'Get Patient By LastName & Type: ' + CAST(@PatientID AS VARCHAR(20))
	IF @PatientID IS NOT NULL
		EXEC InsertAuditSingleRead @ReadDate, @PatientID, 5, @ModifiedBy, @ReadRecID
	----------------------------------------------------------

	IF(LOWER(@PersonalType) = LOWER('TECHNICIAN') OR LOWER(@PersonalType) = LOWER('PROVIDER'))
	BEGIN
		SELECT a.ID, 
		d.Value AS PersonalType, 
		a.FirstName, 
		a.MiddleName, 
		a.LastName,
		a.DateOfBirth, 
		a.Demographic, 
		a.EADStopDate,
		a.IsPOC,
		b.SiteCode as 'SiteCodeID',
		a.Comments,
		a.IsActive,
		a.TheaterLocationCode, 
		a.ModifiedBy, 
		a.DateLastModified
		FROM dbo.Individual a
		INNER JOIN dbo.IndividualIDSiteCodeUnion b on a.ID = b.IndividualID
		INNER JOIN dbo.IndividualType c ON a.ID = c.IndividualID and c.TypeID = @TypeID
		INNER JOIN dbo.LookupTable d ON d.Value = @PersonalType
		WHERE a.LastName LIKE @TempSearch AND 
			c.TypeID = @TypeID
		ORDER BY a.LastName, a.FirstName ASC	
	END
	ELSE
	BEGIN
		SELECT a.ID, 
		d.Value as PersonalType, 
		FirstName, 
		MiddleName, 
		LastName,
		DateOfBirth, 
		Demographic, 
		EADStopDate,
		IsPOC,
		SiteCodeID,
		Comments,
		a.IsActive,
		TheaterLocationCode, 
		a.ModifiedBy, 
		a.DateLastModified
		FROM dbo.Individual a
		INNER JOIN dbo.IndividualType c ON a.ID = c.IndividualID and c.TypeID = @TypeID
		INNER JOIN dbo.LookupTable d ON d.Value = @PersonalType
		WHERE LastName LIKE @TempSearch AND 
			d.ID = @TypeID
		ORDER BY LastName, FirstName ASC	
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualByIndividualID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualByIndividualID] 

@ID		INT,
@IsActive	bit = 1,  -- Remove at a later time
@ModifiedBy VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	----  Audit Single Read -- 10/15/14 - BAF
	Declare @ReadDate	DateTime = GetDate()
	Declare @RecID		VarChar(20) = Cast(@ID as VarChar(20))
	
	Exec InsertAuditSingleRead @ReadDate, @ID, 5, @ModifiedBy, @RecID
	
	-----------------

	SELECT a.ID, 
	lt.Value as PersonalType, 
	a.FirstName, 
	a.LastName, 
	a.MiddleName, 
	a.DateOfBirth, 
	a.Demographic, 
	a.EADStopDate,
	a.IsPOC,
	a.TheaterLocationCode,
	a.SiteCodeID,
	a.IsActive, 
	a.Comments,
	a.ModifiedBy,
	a.DateLastModified,
	b.IDNumber,
	b.IDNumberType,
	a.NextFOCDate
	FROM dbo.Individual a
	INNER JOIN dbo.IndividualIDNumber b ON a.ID = b.IndividualID
	INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID
	INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.ID
	WHERE a.ID = @ID --and b.IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualByIDNumberAndIDNumberType]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 5 Nov 2014
-- Description:	Get an individual based on their ID Number, ID Number Type.
--				This could be used to determine if there is already an individual record with that information.
-- MODIFIED:  7 Nov 2014 to use the IndividualType table for looking up the PersonalType - BAF
--			29 Oct 2015 removed IndividualType to return only one row per individual - BAF
--          jrp modified so that the IndividualIDNumber needs to be active like the Individual
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualByIDNumberAndIDNumberType] 
@IDNumber		varchar(15),
@IDNumberType	varchar(3),
@ModifiedBy		VarChar(200),
@IsPatient		BIT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TempID INT,
			@TempIsPatient INT
			
	SELECT @TempID = 0
	
	-- Set TempID to the IndividualID (Key of Individual) based on IDNumber, IDNumberType
	SELECT @TempID = i.ID
	FROM dbo.Individual i
	INNER JOIN dbo.IndividualIDNumber id on i.ID = id.IndividualID AND id.IsActive = i.IsActive
--	INNER JOIN dbo.IndividualType it ON i.ID = it.IndividualID
	WHERE 
	id.IDNumber = @IDNumber AND 
	id.IDNumberType = @IDNumberType
	
	-- Set the @IsPatient flag
	SELECT @IsPatient = 1
	FROM Individual i
	INNER JOIN IndividualIDNumber idn on i.ID = idn.IndividualID AND idn.IsActive = i.IsActive
	INNER JOIN IndividualType it on i.ID = it.IndividualID
	WHERE i.ID = @TempID AND it.TypeID = 110
	
	--IF @TempIsPatient = 110
	--BEGIN
	--	SELECT @IsPatient = 1
	--END
	
	----- Audit SingleRead - 10/17/14 - BAF
	Declare @ReadDate	DateTime = GetDate()
	Declare @PatientList	VARCHAR(Max)
	Declare @Notes		VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE (@PatientList, 'Individual Record ID: ') + CAST(i.ID AS VARCHAR(20)) + ', '
	FROM dbo.Individual i
		INNER JOIN dbo.IndividualIDNumber id on i.ID = id.IndividualID AND id.IsActive = i.IsActive
	WHERE i.ID = @TempID  
		AND id.IDNumber = @IDNumber
		
	SELECT @Notes = 'List of Individuals with a given ID Number and Type'
	
	IF @PatientList IS NOT null
		EXEC dbo.InsertAuditMultiRead @ReadDate,@ModifiedBy, 5, @PatientList, @Notes
	----------
	
	IF(@TempID IS NOT NULL)
	BEGIN
		SELECT Comments, 
		i.DateLastModified, 
		DateOfBirth, 
		Demographic, 
		EADStopDate, 
		FirstName, 
		i.ID, 
		i.IsActive, 
		IsPOC, 
		LastName, 
		TheaterLocationCode, 
		MiddleName, 
		i.ModifiedBy, 
	--	lt.Value AS PersonalType,
		SiteCodeID,
		IDNumber,
		IDNumberType,
		@IsPatient as IsPatient
		FROM dbo.Individual i
		INNER JOIN dbo.IndividualIDNumber id on i.ID = id.IndividualID AND id.IsActive = i.IsActive
--		INNER JOIN dbo.IndividualType it ON i.ID = it.IndividualID
--		INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.ID
		WHERE i.ID = @TempID 
			AND id.IDNumber = @IDNumber
			AND id.IDNumberType = @IDNumberType
			--And it.TypeID = 110

	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualByIDNumber]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualByIDNumber] 
@IDNumber	varchar(15),
@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TempID int
		
	SELECT @TempID = IndividualID FROM dbo.IndividualIDNumber 
	WHERE IDNumber = @IDNumber

	------ Auditing Single Read - 10/17/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @Patient int = @TempID
	Declare @RecID VarChar(50) = Cast(@TempID as VarChar(50))
	
	Exec InsertAuditSingleRead @ReadDate, @Patient, 5, @ModifiedBy, @RecID
	---------------------- 
	
	IF(@TempID IS NOT NULL)
	BEGIN
		SELECT a.ID,
		lt.Value AS PersonalType,
		FirstName,
		MiddleName,
		LastName,
		DateOfBirth,
		Demographic,
		EADStopDate,
		IsPOC,
		SiteCodeID,
		Comments,
		a.IsActive,
		TheaterLocationCode,
		a.ModifiedBy, 
		a.DateLastModified,
		b.IDNumber,
		b.IDNumberType
		FROM dbo.Individual a
			INNER JOIN dbo.IndividualIDNumber b ON a.ID = b.IndividualID
			INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID
			INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.id
		WHERE a.ID = @TempID
			AND b.IDNumber = @IDNumber
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetIndividualAddressesByIndividualID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  30 Oct 2017 - baf - Added Address Verification
-- =============================================
CREATE PROCEDURE [dbo].[GetIndividualAddressesByIndividualID] 
@IndividualID int,
@IsActive bit = 1,
@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-------- Audit MultiRead -- 10/17/14 - BAF
	--- This SP has one patient but may have multiple records.
	Declare @ReadDate DateTime	= GetDate()
	Declare @PatientList VarChar(20) = Cast(@IndividualID as VarChar(20))
	Declare @Notes		VarChar(Max)
	
	Select @Notes = Coalesce (@Notes, 'Address Record IDs: ') + CAST(ID as VarChar(20)) + ', '
	From dbo.IndividualAddress where IndividualID = @IndividualID
	
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 6, @PatientList, @Notes
	------------------------

	IF(@IsActive = 1)
	BEGIN
		SELECT ID,
		IndividualID, 
		Address1, 
		Address2,
		Address3,
		City,
		[State],
		Country,
		ZipCode, 
		AddressType,
		UIC, 
		IsActive, 
		ModifiedBy,
		DateLastModified,
		DateVerified,
		VerifiedBy,
		ExpireDays
		FROM dbo.IndividualAddress
		WHERE IndividualID = @IndividualID
--		AND IsActive = 1
	END
	ELSE
	BEGIN
		SELECT ID,
		IndividualID, 
		Address1, 
		Address2,
		City,
		[State],
		Country,
		ZipCode, 
		AddressType,
		UIC, 
		IsActive, 
		ModifiedBy,
		DateLastModified,
		DateVerified,
		VerifiedBy,
		ExpireDays
		FROM dbo.IndividualAddress
		WHERE IndividualID = @IndividualID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetExamsByIndividualID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetExamsByIndividualID] 
@IndividualID		int,
@ModifiedBy			VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--------------- Audit Multi Record Read -- 10/14/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @PatientList VarChar(500) = Cast(@IndividualID as VarChar(20))
	Declare @Notes VarChar(max)

	Select @Notes = Coalesce (@Notes,'EXAM RECORD IDs: ') +  Cast(ID as VarChar(20)) + ', ' 
	from dbo.Exam where IndividualID_Patient = @IndividualID
	
	If @Notes is Not Null 
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 10, @PatientList, @Notes
	
	----------------------

	SELECT 	ID, 
		IndividualID_Patient,
		--AddressID_Patient,
		IndividualID_Examiner,
		ODCorrectedAcuity,
		ODUncorrectedAcuity,
		OSCorrectedAcuity,
		OSUncorrectedAcuity,
		ODOSCorrectedAcuity,
		ODOSUncorrectedAcuity,
		Comment,
		ExamDate,
		ModifiedBy,
		DateLastModified
	FROM dbo.Exam
	WHERE IndividualID_Patient = @IndividualID
	ORDER BY ExamDate desc
END
GO
/****** Object:  StoredProcedure [dbo].[GetExamByExamID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  5/1/15 - baf - to remove AddressID_Patient
-- =============================================
CREATE PROCEDURE [dbo].[GetExamByExamID] 
@ID		int,
@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Auditing Read Routine - 10/14/14 - BAF
	Declare @ReadDate VarChar(20) = Cast(GetDate() as VarChar(20))
	Declare @PatientID int
	Declare @RecID VarChar(20) = Cast(@ID as VarChar(20))

	Select @PatientID = IndividualID_Patient from dbo.Exam where ID = @ID

	EXEC InsertAuditSingleRead @ReadDate, @PatientID, 10, @ModifiedBy, @RecID
	
	----------------

	SELECT 	ID, 
		IndividualID_Patient,
		--AddressID_Patient,
		IndividualID_Examiner,
		ODCorrectedAcuity,
		ODUncorrectedAcuity,
		OSCorrectedAcuity,
		OSUncorrectedAcuity,
		ODOSCorrectedAcuity,
		ODOSUncorrectedAcuity,
		Comment,
		ExamDate,
		ModifiedBy,
		DateLastModified
	FROM dbo.Exam
	WHERE ID = @ID
	ORDER BY ExamDate Desc
END
GO
/****** Object:  StoredProcedure [dbo].[GetAuthorizationsByUserName]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[GetAuthorizationsByUserName]
	@UserName nvarchar(256) 
AS
BEGIN
--	SET NOCOUNT ON;
	
	--Declare @UserName nvarchar(256)
	--Set @UserName = 'clinictech'

	IF Exists
	(
	--Check if the @UserName parameter provided is an "SSO_UserName" id.
	SELECT [dbo].[Authorization].UserName
	FROM [dbo].[Authorization]
	WHERE [dbo].[Authorization].SSO_UserName = @UserName
	)
	BEGIN
		--If the @UserName parameter is a SSO_UserName return all its associated aspnet accounts.
		SELECT Distinct a.*, RoleNames as RoleName
		FROM [dbo].[Authorization] a 
		join [dbo].[aspnet_Users] u on a.UserName = u.LoweredUserName
		join [dbo].[aspnet_UsersInRoles] ri on u.UserId=ri.UserId
		join [dbo].[aspnet_Membership] m on u.UserId=m.UserId	
		cross apply (  SELECT STUFF((SELECT ',' + r.RoleName 
    		                            FROM aspnet_UsersInRoles rs
    		                            left outer join aspnet_Roles r on rs.RoleId=r.RoleId
    		                            WHERE rs.UserId =ri.UserId
    		                            FOR XML PATH('')),1,1,'')
 
    		         ) d (RoleNames)
		WHERE a.SSO_UserName = @UserName and m.IsApproved != 0 and m.IsLockedOut = 0
			  
	END
	ELSE 
	BEGIN
		--Else use the @UserName parameter is an aspnet account name return all other accounts associated by an SSO_UserName 
		SELECT Distinct a.*, RoleNames as RoleName
		FROM [dbo].[Authorization] a
		join [dbo].[aspnet_Users] u on a.UserName = u.LoweredUserName
		join [dbo].[aspnet_UsersInRoles] ri on u.UserId=ri.UserId
		join [dbo].[aspnet_Membership] m on u.UserId=m.UserId		
		cross apply (   SELECT STUFF((SELECT ',' + r.RoleName 
    		                            FROM aspnet_UsersInRoles rs
    		                            left outer join aspnet_Roles r on rs.RoleId=r.RoleId
    		                            WHERE rs.UserId =ri.UserId
    		                            FOR XML PATH('')),1,1,'')
    		         ) d (RoleNames)		
		WHERE 
		m.IsApproved != 0 and m.IsLockedOut = 0 and
		(
		a.UserName = @UserName 
		or (a.SSO_UserName is not null
			and a.SSO_UserName not in ( '' )
			and a.SSO_UserName in (			
									 SELECT Distinct [dbo].[Authorization].SSO_UserName
									 FROM [dbo].[Authorization]
									 WHERE [dbo].[Authorization].UserName = @UserName
			   					   ) 
			)				
		)
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetAuthorizationsByCAC_ID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
	Modified:  11/21/2016 - baf - To remove Site Information
*/
CREATE PROCEDURE  [dbo].[GetAuthorizationsByCAC_ID]
	@CAC_ID nvarchar(256),
	@issuerName nvarchar(256)

AS

	SET NOCOUNT ON;

BEGIN
-- log entries
--insert into SQLErrorLog ([ModifiedBy]
--      ,[ErrorNumber]
--      ,[ErrorSeverity]
--      ,[ErrorState]
--      ,[ErrorProcedure]
--      ,[ErrorLine]
--      ,[ErrorMessage]
--      ,[ErrorDate])
--values
--('templog',1,1,1,'GetAuthorizationsByCAC_ID',1,'cacid['+@CAC_ID+'] issuer['+@issuerName+']',GETDATE())

	--DECLARE @CAC_ID varchar(256)
	--SET @CAC_ID= '1014240310'
	--Declare @issuerName as varchar(256)
	--SET @issuerName='DOD CA-31'
Select UserName, RoleName, CacID, CacIssuer --,SiteCode
From (
	SELECT Distinct a.UserName, RoleNames as RoleName, a.CacId, a.CacIssuer, IsApproved, IsLockedOut --, a.SiteCode, 
	FROM [dbo].[Authorization] a
	join [dbo].[aspnet_Users] u on a.UserName = u.UserName
	join [dbo].[aspnet_UsersInRoles] ri on u.UserId=ri.UserId
	join [dbo].[aspnet_Membership] m on u.UserId=m.UserId
	cross apply (  SELECT STUFF((SELECT ',' + r.RoleName 
    		                     FROM aspnet_UsersInRoles rs
    		                     left outer join aspnet_Roles r on rs.RoleId=r.RoleId
    		                     WHERE rs.UserId =ri.UserId
    		                     FOR XML PATH('')),1,1,'')
    	         ) d (RoleNames)	
	WHERE m.IsApproved = 1 and m.IsLockedOut = 0 and 
		(a.CacId = @CAC_ID and a.CacIssuer = @issuerName)
		or (a.SSO_UserName is not null
		and a.SSO_UserName not in ( '' )
		and a.SSO_UserName in (			
								 SELECT Distinct a.SSO_UserName
								 FROM [dbo].[Authorization] a
								 WHERE a.CacId = @CAC_ID and a.CacIssuer = @issuerName
			   				   ) 
		   )	
) x
where IsApproved = 1 and IsLockedOut = 0
END
GO
/****** Object:  StoredProcedure [dbo].[GetPrescriptionByID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  16 March 2018 - baf - Added ScannedRx ID
-- =============================================
CREATE PROCEDURE [dbo].[GetPrescriptionByID] 
@ID	INT,
@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	---------- Audit Single Read 10/17/14 - BAF
	Declare @DateRead DateTime = GetDate()
	Declare @PatientID int
	Declare @ReadRecID VarChar(200) = Cast(@ID as VarChar(200))
	
	Select @PatientID = IndividualID_Patient FROM dbo.Prescription WHERE ID = @ID
	If @PatientID is not null
		Exec InsertAuditSingleRead @DateRead, @PatientID, 11, @ModifiedBy, @ID
	----------------------------------

	SELECT ID,
	Case when ExamID Is Null then 0 else ExamID End ExamID,
	IndividualID_Patient,
	IndividualID_Doctor,
	ODSphere,
	OSSphere,
	ODCylinder,
	OSCylinder,
	--ODAxis,
	RIGHT('000' + CAST(ODAxis AS VarCHAR(3)),3) AS ODAxis,
	RIGHT('000' + CAST(OSAxis AS VarCHAR(3)),3) AS OSAxis,
	--OSAxis,
	ODHPrism,
	OSHPrism,
	ODVPrism,
	OSVPrism,
	ODHBase,
	OSHBase,
	ODVBase,
	OSVBase,
	ODAdd,
	OSAdd,
	EnteredODSphere,
	EnteredOSSphere,
	EnteredODCylinder,
	EnteredOSCylinder,
	EnteredODAxis,
	EnteredOSAxis,
	PrescriptionDate,
	IsActive,
	IsUsed,
	ModifiedBy,
	DateLastModified,
	PDNear,
	PDDistant,
	ODPDDistant,
	ODPDNear,
	OSPDDistant,
	OSPDNear,
	Case when IsMonoCalculation is Null then 0 else IsMonoCalculation end IsMonoCalculation,
	RxName,
	RxScanID
	FROM dbo.Prescription
	WHERE ID = @ID

END
GO
/****** Object:  StoredProcedure [dbo].[InsertExam]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Shear
-- Create date: 11 Oct 2011
-- Description:	Insert and exam using Patient and Examiner ID's
-- =============================================
CREATE PROCEDURE [dbo].[InsertExam] 
@IndividualID_Patient		int,
--@AddressID_Patient			INT,
@IndividualID_Examiner		int,
@ODCorrectedAcuity			VARCHAR(12),
@ODUncorrectedAcuity		VARCHAR(12),
@OSCorrectedAcuity			VARCHAR(12),
@OSUncorrectedAcuity		VARCHAR(12),
@ODOSCorrectedAcuity		VARCHAR(12),
@ODOSUncorrectedAcuity		VARCHAR(12),
@Comment					VARCHAR(256),
@ExamDate					DateTime,
@IsActive					BIT,
@ModifiedBy					VARCHAR(200),
@Success					INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT 
		
	BEGIN TRY
		BEGIN TRANSACTION

			INSERT INTO dbo.Exam
				(IndividualID_Patient,
				--AddressID_Patient,
				IndividualID_Examiner,
				ODCorrectedAcuity,
				ODUncorrectedAcuity,
				OSCorrectedAcuity,
				OSUncorrectedAcuity,
				ODOSCorrectedAcuity,
				ODOSUncorrectedAcuity,
				Comment,
				ExamDate,
				IsActive,
				ModifiedBy,
				DateLastModified,
				LegacyID)
			VALUES (@IndividualID_Patient,
			--@AddressID_Patient,
			@IndividualID_Examiner,
			@ODCorrectedAcuity,
			@ODUncorrectedAcuity,
			@OSCorrectedAcuity,
			@OSUncorrectedAcuity,
			@ODOSCorrectedAcuity,
			@ODOSUncorrectedAcuity,
			@Comment,
			@ExamDate,
			@IsActive,
			@ModifiedBy,
			GETDATE(),
			0)
			
			SELECT @TempID = @@IDENTITY
			
			--Add Audting -- 10/7/2014 BAF
			Declare @DateLastModified DateTime = GetDate()
			Declare @ExamRec VarChar(Max) = (Cast(@IndividualID_Patient as VarChar(20)) --+ ', ' + CAST(@AddressID_Patient AS VARCHAR(20)) 
				+ ', ' + Cast(@IndividualID_Examiner as VarChar(20)) + ', ' + @ODCorrectedAcuity + ', ' + @ODUncorrectedAcuity + ', ' + 
				@OSCorrectedAcuity + ', ' + @OSUncorrectedAcuity + ', ' + @ODOSCorrectedAcuity + ', ' + @ODOSUncorrectedAcuity + ', ' + 
				@Comment + ', ' + Cast(@ExamDate as VarChar(20))+ ', ' + Cast(@IsActive as Char(1)) + ', ' + @ModifiedBy)
			Declare @RecID VarChar(20) = Cast(@TempID as VarChar(20))
			
			If @RecID is not null
				EXEC InsertAuditTrail @DateLastModified,'ALL',@ExamRec, @RecID, 10, @ModifiedBy,'I'
			---------------	  

		COMMIT TRANSACTION
		--SELECT 	ID,
		--		IndividualID_Patient,
		--		--AddressID_Patient,
		--		IndividualID_Examiner,
		--		ODCorrectedAcuity,
		--		ODUncorrectedAcuity,
		--		OSCorrectedAcuity,
		--		OSUncorrectedAcuity,
		--		ODOSCorrectedAcuity,
		--		ODOSUncorrectedAcuity,
		--		Comment,
		--		ExamDate,
		--		ModifiedBy,
		--		IsActive,
		--		DateLastModified
		--FROM dbo.Exam
		--WHERE ID = @TempID
		
		-- Insert Succeeded
		SET @Success = 1 
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			-- Insert Failed
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			SET @Success = 0
		END
	END CATCH
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[InsertIndividualSiteCode]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 29 Oct 2014
-- Description:	Inserts a record into the IndividualIdSiteCodeUnion union table
-- Modified:  16 Nov 2016 - baf - To delete existing records and insert new
--		records from list of sites provided for this user.
-- =============================================
CREATE PROCEDURE [dbo].[InsertIndividualSiteCode]
	-- Add the parameters for the stored procedure here
	@IndividualId	int,
	@SiteCode		varchar(max),
	@Success	INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID INT = 0,
		@ID INT, @RecCnt INT = 0, @Site VarChar(6)
	
	CREATE TABLE #SiteCodes (RowID INT, SiteCode VARCHAR(6))
	
	--  Load the temp table with site codes for this individual
	BEGIN
		INSERT INTO #SiteCodes (RowID, SiteCode)
		SELECT * FROM dbo.SplitStrings(@SiteCode,',')

		SELECT @RecCnt = Max(RowID) from #SiteCodes
	END
	
	-- Commented out 11/16/2016 to provide for delete/replace of all site codes for the individual
	--IF NOT EXISTS (SELECT * FROM dbo.IndividualIDSiteCodeUnion s WHERE s.IndividualID = @IndividualId AND s.SiteCode = @SiteCode)
	--BEGIN
	--	INSERT INTO dbo.IndividualIDSiteCodeUnion (IndividualID, SiteCode) VALUES (@IndividualId, @SiteCode)
	--END

	BEGIN TRY
		BEGIN TRANSACTION
			-- Delete all existing sitecodes for the individual
			BEGIN
				DELETE FROM dbo.IndividualIDSiteCodeUnion where IndividualID = @IndividualID
			END

			DECLARE @Cnt int = 1

			WHILE @RecCnt >= @Cnt
				BEGIN
					IF @RecCnt >= 1
					BEGIN
					-- Begin inserting from sitecode list
						SELECT @Site = SiteCode from #SiteCodes where RowID = @Cnt

						INSERT INTO dbo.IndividualIDSiteCodeUnion (IndividualID, SiteCode)
						VALUES (@IndividualID, @Site)
				
						Set @Cnt = @Cnt + 1
					END
				END	
			SET @Success = 1
		COMMIT TRANSACTION

		DROP TABLE #SiteCodes
		
	END TRY	
	BEGIN CATCH
		IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		SET @Success = 0
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertIndividualPhoneNumber]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertIndividualPhoneNumber] 
@IndividualID		INT,
@PhoneNumberType	VARCHAR(5)= '',
@AreaCode			VARCHAR(3) = '000',
@PhoneNumber		VARCHAR(15)= '000000000000000',
@Extension			VARCHAR(7)= '',
@IsActive			BIT = true,
@ModifiedBy			VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT		

	UPDATE dbo.IndividualPhoneNumber
	SET IsActive = 0
	WHERE IndividualID = @IndividualID
	
	BEGIN TRY
		BEGIN TRANSACTION
		
			INSERT INTO dbo.IndividualPhoneNumber
			(AreaCode, 
			DateLastModified, 
			Extension, 
			IndividualID, 
			IsActive, 
			ModifiedBy, 
			PhoneNumber, 
			PhoneNumberType,
			LegacyID)
			VALUES
			(@AreaCode, 
			GETDATE(), 
			@Extension, 
			@IndividualID, 
			@IsActive, 
			@ModifiedBy, 
			replace(@PhoneNumber,'-',''), 
			@PhoneNumberType,
			0)
		
		----Auditing Routines added 10/7/14 BAF
		Declare @DateLastModified DateTime = GetDate()
		Declare @PhoneRecID VarChar(50) = Cast(Scope_Identity() as VarChar(50))
		Declare @PhoneInfo VarChar(Max) = (Cast(@IndividualID as VarChar(15)) + ', ' + @PhoneNumberType + ', ' + 
			@AreaCode + ', ' + @PhoneNumber + ', ' + @Extension + ', ' + Cast(@IsActive as VarChar(1))
			 + ', ' + @ModifiedBy + ', ' + Cast(@DateLastModified as VarChar(20)))
		
		 If @PhoneRecID is not null
			Exec InsertAuditTrail @DateLastModified,'ALL Phone Number',@PhoneInfo,@PhoneRecID, 7, @ModifiedBy,'I'	
		-------------------------------------------------------------------

		COMMIT TRANSACTION

			SELECT AreaCode, 
			DateLastModified, 
			Extension, 
			ID, 
			IndividualID, 
			IsActive, 
			ModifiedBy, 
			PhoneNumber, 
			PhoneNumberType 
			FROM dbo.IndividualPhoneNumber
			WHERE
			IndividualID = @IndividualID
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertIndividualIDNumber]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertIndividualIDNumber]
@IndividualID		int,
@IDNumber			varchar(15),
@IDNumberType		varchar(3),
@IsActive			bit = true,
@ModifiedBy			varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TempID INT
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	--Used to ensure that FSN's are stored as SSNs
	IF (Len(@IDNumber) = '9') and (Left(@IDNumber,1) = '9') and (@IDNumberType <> 'SSN')
		Begin
			Set @IDNumberType = 'SSN'
		End
	
	UPDATE dbo.IndividualIDNumber
	SET IsActive = 0
	WHERE IndividualID = @IndividualID and IDNumberType = @IDNumberType
--		and IDNumberType = @IDNumberType -- Added 9/18/14 BAF -- removed, all ids are set inactive, new id is active

	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO dbo.IndividualIDNumber
			(IndividualID, IDNumber, IDNumberType, IsActive, ModifiedBy, DateLastModified, LegacyID)
			VALUES(@IndividualID, @IDNumber, @IDNumberType, 1, @ModifiedBy, GETDATE(), 0)

			SELECT @TempID = @@IDENTITY
		COMMIT TRANSACTION
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
				
		--------Auditing Routines added 10/7/14 - BAF
		Declare @DateLastModified DateTime = GetDate()
		Declare @RecID VarChar(50) = Cast(@TempID as VarChar(50))
		Declare @IdNbrInfo VarChar(Max) = (Cast(@IndividualID as VarChar(20)) + ', ' + @IDNumber  + ', ' + @IDNumberType + ', 1,' 
			+ @ModifiedBy  + ', '+ Cast(@DateLastModified as VarChar(20)))
					
		If @RecID is not null
			Exec InsertAuditTrail @DateLastModified,'ALL ID Number', @IdNbrInfo, @RecID, 21, @ModifiedBy,'I'
		-------------------
	
			SELECT ID, 
			IndividualID, 
			IDNumber, 
			IDNumberType, 
			IsActive, 
			ModifiedBy, 
			DateLastModified 
			FROM dbo.IndividualIDNumber
			WHERE ID = @TempID
END
GO
/****** Object:  StoredProcedure [dbo].[InsertIndividualEMailAddress]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertIndividualEMailAddress] 
@IndividualID	int,
@EMailType		varchar(10) = 'HOME',
@EMailAddress	varchar(200) = 'mail.mail@mail.mail',
@IsActive		bit = 1,
@ModifiedBy		varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0

	UPDATE dbo.IndividualEMailAddress
	SET IsActive = 0
	WHERE IndividualID = @IndividualID	

	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO dbo.IndividualEMailAddress 
			(IndividualID, EMailType, EMailAddress, IsActive, ModifiedBy, DateLastModified, LegacyID)
			VALUES(@IndividualID, @EMailType, @EMailAddress, @IsActive, @ModifiedBy, GETDATE(), 0)
			
			--Auditing Routines added 10/7/14 - BAF
			Declare @EmailAddrID VarChar(50) = 'EmailAddress ID: ' +  Cast(Scope_Identity() as VarChar(50))
			Declare @DateLastModified DateTime = GetDate()
			Declare @EmailAddrInfo VarChar(Max) = (Cast(@IndividualID as VarChar(15)) + ', ' + @EmailType + ', ' + @EmailAddress
				 + ', ' + Cast(@IsActive as VarChar(1)) + ', ' + @ModifiedBy + ', ' + Cast(@DateLastModified as VarChar(20)))
			
			If @EmailAddrID is not null
				Exec InsertAuditTrail @DateLastModified,'ALL Email Address',@EmailAddrInfo, @EmailAddrID, 9, @ModifiedBy, 'I'
			-----------
		COMMIT TRANSACTION

		SELECT ID, 
		IndividualID, 
		EMailType, 
		EMailAddress, 
		IsActive, 
		ModifiedBy, 
		DateLastModified
		FROM dbo.IndividualEmailAddress
		WHERE IndividualID = @IndividualID
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertIndividualAddress]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  10/2/17 - baf - Added Address Verification ability
-- =============================================
CREATE PROCEDURE [dbo].[InsertIndividualAddress] 
@IndividualID	int,
@Address1		varchar(200),
@Address2		varchar(200) = '',
@Address3		varchar(200) = '',
@City			varchar(200),
@State			varchar(2),
@Country		varchar(2),
@ZipCode		varchar(10),
@AddressType	varchar(10),
@IsActive		bit = 1,
@UIC			varchar(6) = '',
@ModifiedBy		varchar(200),
@ExpireDays		INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	UPDATE dbo.IndividualAddress
	SET IsActive = 0
	WHERE IndividualID = @IndividualID
	
	DECLARE @TempID INT = 0
	
	BEGIN TRY
		BEGIN TRANSACTION
		if(@UIC is null)
		begin
			INSERT INTO dbo.IndividualAddress
			(IndividualID, Address1, Address2, Address3, City, [State], Country, ZipCode, addressType, IsActive, ModifiedBy, UIC, DateLastModified, LegacyID,
			DateVerified, VerifiedBy, ExpireDays)
			VALUES(@IndividualID, @Address1, @Address2, @Address3, @City, @State, @Country, @ZipCode, @AddressType, @IsActive, @ModifiedBy, null, GETDATE(), 0,
			GETDATE(),@ModifiedBy, @ExpireDays)
		end
		else
		begin
			INSERT INTO dbo.IndividualAddress
			(IndividualID, Address1, Address2, Address3, City, [State], Country, ZipCode, addressType, IsActive, ModifiedBy, UIC, DateLastModified, LegacyID,
			DateVerified, VerifiedBy, ExpireDays)
			VALUES(@IndividualID, @Address1, @Address2, @Address3, @City, @State, @Country, @ZipCode, @AddressType, @IsActive, @ModifiedBy, @UIC, GETDATE(), 0,
			GETDATE(), @ModifiedBy, @ExpireDays)
		end

		SET @TempID = @@IDENTITY
		
		
		COMMIT TRANSACTION
		
		
	
		-- Auditing added 10/7/2014 - BAF
		Declare @PatAddrID VarChar(50) = Cast(SCOPE_IDENTITY() as varChar(50))
		Declare @DateLastModified DateTime = GetDate()
		
		Declare @PatAddr VarChar(Max) = (Cast(@IndividualID as VarChar(15)) + ', ' + @Address1 + ', ' + @Address2 + ', ' + @Address3 + ', ' + @City
			 + ', ' + @State + ', ' + @Country + ', ' + @ZipCode + ', ' + @AddressType + ', ' + Cast(@IsActive as VarChar(1)) + ', ' + @ModifiedBy 
			 + ', ' + @UIC + ', ' + Cast(@DateLastModified as VarChar(20)))
			 
		If @PatAddrID is not null
			Exec InsertAuditTrail @DateLastModified, 'ALL Individual Address',@PatAddr,@PatAddrID,6,@ModifiedBy,'I'
		-------------------
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH

		SELECT ID, IndividualID, Address1, Address2, Address3, City, [State], Country, ZipCode, AddressType, UIC, IsActive, ModifiedBy, DateLastModified,
			VerifiedBy, DateVerified, ExpireDays
		FROM dbo.IndividualAddress
		WHERE IndividualID = @IndividualID
END
GO
/****** Object:  StoredProcedure [dbo].[SearchForPatients]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/17/2015
-- Description:	Revision to Individual Search
-- Modified:  	9/12/18 - Increased First and Last Name fields in temp table to 75 char
-- =============================================
CREATE PROCEDURE [dbo].[SearchForPatients]
	-- Add the parameters for the stored procedure here
@IDNumber		varchar(15) = Null,
@LastName		varchar(75) = Null,
@FirstName		VARCHAR(75) = NULL,
@SiteCodeID		VARCHAR(6) = Null,
@PersonalType	VARCHAR(25) = 'ALL', -- If no personal type is selected, get everything
@IsActive		BIT = 1,
@ModifiedBy		VarChar(200),
@RecCnt			INT  OUTPUT

As
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @TypeID Int,
		@TempID VarChar(15)= Null,
		@TempLName VarChar(75) = NULL,
		@TempFName VarChar(75) = Null
	
	IF (LOWER(@PersonalType) = 'all')
		BEGIN
			Set @TypeID = Null;
		END
	ELSE
		BEGIN
			SELECT @TypeID = ID FROM dbo.LookupTable
			WHERE Code = 'IndividualType' AND Value = @PersonalType;
		END
		
	IF(@IDNumber IS NOT NULL)
	BEGIN
		SET @IDNumber = REVERSE(@IDNumber);
		Set @TempID = @IDNumber + '%';
	END

	IF (@LastName is not null)
		Begin
			Set @TempLName = @LastName + '%';
		END
		
	IF (@FirstName IS NOT NULL)
	BEGIN
		SET @TempFName = @FirstName + '%';
	End


		Create Table #tempSearch (RowNbr INT, ID int, TypeID int, PersonalType Varchar(25), FirstName VarChar(75),
			 MiddleName VarChar(20), LastName VarChar(75), DateOfBirth Date, Demographic VarChar(8),
			EADStopDate Date, IsPOC bit, TheaterLocationCode VarChar(10), SiteCodeID VarChar(6),
			IsActive Bit, Comments VarChar(256), ModifiedBy VarChar(200), DateLastModified Date,
			IDNumber VarChar(15), IDNumberType VarChar(3), IDNumberReverse VarChar(15), ActiveID int)
			
		Create Index IX_OrderNbrStat on #tempSearch (LastName) Include (FirstName, MiddleName, IDNumber);
		
		If @LastName is null
		Begin
		Insert into #tempSearch	
				Select DISTINCT ROW_NUMBER() OVER (ORDER BY a.ID) AS RowNbr ,a.ID, it.TypeID, lt.value as PersonalType, 
					a.FirstName, a.MiddleName, a.LastName,
					a.DateOfBirth, a.Demographic, a.EADStopDate, a.IsPOC, a.TheaterLocationCode, a.SiteCodeID,
					a.IsActive, a.Comments, a.ModifiedBy, a.DateLastModified, b.IDNumber, b.IDNumberType,
					b.IDNumberReverse, b.IsActive as ActiveID
				From dbo.Individual a
					Left Join dbo.IndividualIDNumber b on a.ID = b.IndividualID	
					Inner Join (SELECT IndividualID,TypeID FROM dbo.IndividualType 
						WHERE (@TypeID IS NULL OR TypeID = @TypeID)) it ON a.ID = it.IndividualID
					Left Join dbo.LookupTable lt on lt.ID = it.TypeID
					--Left Join dbo.IndividualIDSiteCodeUnion c on a.ID = c.IndividualID
				where  a.IsActive = 1  AND b.IsActive = 1 AND (@TypeID is Null OR it.TypeID = @TypeID)
					and (@IDNumber is null or IDNumberReverse like @TempID) and it.IndividualID = a.ID 
					and ((@SiteCodeID is Null or SiteCodeID = @SiteCodeID)) --or (@SiteCodeID is Null or c.SiteCode = @SiteCodeID))
		END
		ELSE
		BEGIN
			Insert into #tempSearch	
				Select DISTINCT ROW_NUMBER() OVER(ORDER BY a.ID) AS RowNbr, a.ID, it.TypeID, lt.value as PersonalType, 
					a.FirstName, a.MiddleName, a.LastName,
					a.DateOfBirth, a.Demographic, a.EADStopDate, a.IsPOC, a.TheaterLocationCode, a.SiteCodeID,
					a.IsActive, a.Comments, a.ModifiedBy, a.DateLastModified, b.IDNumber, b.IDNumberType,
					b.IDNumberReverse, b.IsActive as ActiveID--, c.SiteCode as UnionSite
				From dbo.Individual a
					Left Join dbo.IndividualIDNumber b on a.ID = b.IndividualID	
					INNER JOIN (SELECT IndividualID,TypeID FROM dbo.IndividualType 
						WHERE (@TypeID IS NULL OR TypeID = @TypeID)) it ON a.ID = it.IndividualID
					--Inner Join dbo.IndividualType it on a.ID = it.IndividualID
					Left Join dbo.LookupTable lt on lt.ID = it.TypeID
					--Left Join dbo.IndividualIDSiteCodeUnion c on a.ID = c.IndividualID
				where  (@LastName is null or LastName like @TempLName) and a.IsActive = 1 
					AND (@FirstName is null or FirstName like @TempFName)
					AND b.IsActive = 1 AND (@TypeID is Null OR it.TypeID = @TypeID)
					--and (@IDNumber is null or IDNumberReverse like @TempID) and it.IndividualID = a.ID 
					and ((@SiteCodeID is Null or SiteCodeID = @SiteCodeID)) --or (@SiteCodeID is Null or c.SiteCode = @SiteCodeID))					
			END
			
			CREATE TABLE #TempIDs (ID INT, IDNbrs VARCHAR(MAX), IDType VARCHAR(MAX))
			INSERT INTO #TempIDs
			SELECT DISTINCT ID,'','' FROM #tempSearch			
			
			SET @RecCnt = 0;
			SELECT @RecCnt = COUNT(*) FROM #tempSearch
			
			DECLARE @ID INT = 0,
				@cnt INT = 1,
				@IDNbrs VARCHAR(MAX) = '',
				@IDType VARCHAR(MAX)= '',
				@OldID INT = 0,
				@TempIDNbr VARCHAR(4)
				
				
			WHILE (@Cnt <= @RecCnt)
				Begin
					SELECT @ID = ID FROM #tempSearch WHERE RowNbr = @cnt
					IF @Cnt = 1 
						BEGIN
							UPDATE #TempIDs SET IDNbrs = @IDNbrs, IDType = @IDType WHERE ID = @OldID
							SET @IDNbrs = (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
							SET @IDType = (SELECT IDnumberType FROM #tempSearch WHERE RowNbr = @Cnt)
							SET @TempIDNbr = (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
						End
					IF @OldID = @ID 
						BEGIN
							IF @TempIDNbr <> (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
								BEGIN
									SET @IDNbrs = @IDNbrs + ', ' + (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
									SET @IDType = @IDType + ', ' + (SELECT IDnumberType FROM #tempSearch WHERE RowNbr = @Cnt)
									SET @TempIDNbr = (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
								End
						END
					ELSE
						BEGIN
							IF @TempIDNbr <> (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
							Begin
							UPDATE #TempIDs SET IDNbrs = @IDNbrs, IDType = @IDType WHERE ID = @OldID
							SET @IDNbrs = (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
							SET @IDType = (SELECT IDnumberType FROM #tempSearch WHERE RowNbr = @Cnt)
							SET @TempIDNbr = (SELECT RIGHT(IDNumber,4) FROM #tempSearch WHERE RowNbr = @Cnt)
							END
						End
					SET @cnt = @cnt + 1
					SET @OldID = @ID			
				End
				UPDATE #TempIDs SET IDNbrs = @IDNbrs, IDType = @IDType WHERE ID = @OldID
						
			Select Distinct b.ID, FirstName, LastName, MiddleName, DateOfBirth, Demographic, EADStopDate,
				IsPOC, TheaterLocationCode, SiteCodeID, IsActive, Comments, ModifiedBy, DateLastModified, 
				b.IDNbrs, b.IDType, ActiveID--, UnionSite 
			FROM #tempSearch a LEFT JOIN 
			 #TempIDs b ON a.ID = b.ID
			where ((@LastName is null or LastName like @TempLName) and (@FirstName is null or FirstName like @TempFName)
					or (@IDNumber is null or IDNumberReverse like @TempID))
			--	 and (@TypeID is null or TypeID = @TypeID)
				AND a.ActiveID = 1
			Order by LastName, FirstName ASC

			Select @RecCnt= COUNT(Distinct ID) from #tempSearch
	

DROP TABLE #TempIDs
DROP TABLE #tempSearch		
		
END
GO
/****** Object:  StoredProcedure [dbo].[IURulesOfBehavior]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 11 June 2019
-- Description:	Insert/Update the Rules Of Behavior data for a user.
-- =============================================
CREATE PROCEDURE [dbo].[IURulesOfBehavior]
	-- Add the parameters for the stored procedure here
	@UserName VarChar(250),
	@ROBDate DateTime,
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @ErrorLogID int = 0,
		@IndivID Int = 0,
		@RuleDate DateTime

	-- First get IndividualID for this user
	Select @IndivID = IndividualID from dbo.Profile_PrimarySiteUnion where LowerUserName = @UserName

	-- Check to see if the Individual has a ROB Acceptance Date
	Select @RuleDate = ROBAcceptance from dbo.Individual where ID = @IndivID

	Begin Try
		-- With field in the table set to NULL, all transactions will be an update
		-- Individuals are already created and field is set to NULL before first user login
		Begin Transaction
			Update dbo.Individual set ROBAcceptance = @ROBDate Where ID = @IndivID
		Commit Transaction

		Set @Success = 1
	End Try
	Begin Catch
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END

	   -- Insert Failed
		Set @Success = 0 
		RETURN	
	End Catch
END
GO
/****** Object:  StoredProcedure [dbo].[InsertProfilePrimarySiteUnion]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 Nov 2016
-- Description:	This procedure is used to add records
--		to the table, dbo.Profile_PrimarySiteUnion 
-- =============================================
CREATE PROCEDURE [dbo].[InsertProfilePrimarySiteUnion]
	@lowerusername Varchar(30),
	@IndividualID int,
	@IsCMSUser bit,
	@PrimarySiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @CurCnt Int = 0
	Declare @TempID Int
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	SELECT Count(LowerUserName) from dbo.Profile_PrimarySiteUnion where LowerUserName = @lowerusername
	BEGIN TRY
		BEGIN TRANSACTION	
		If @CurCnt = 0
			Begin
				Insert into dbo.Profile_PrimarySiteUnion (LowerUserName, IndividualID, IsCMSUser, PrimarySiteCode)
				Values (@lowerusername, @IndividualID, @IsCMSUser, @PrimarySiteCode)
			End
		Else
			Begin
				Update dbo.Profile_PrimarySiteUnion
				Set IsCMSUser = @IsCMSUser, PrimarySiteCode = @PrimarySiteCode
				where LowerUserName = @lowerusername
			End
				SELECT @TempID = @@IDENTITY
		COMMIT TRANSACTION
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[InsertPrescription]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:   2/12/15 - BAF - To include PD Changes
--			COA1 Changes - BAF
--			1/28/16 - BAF - Added @RxID as an output parameter
--			16 Mar 2018 - baf - Added Scanned Rx ID
-- =============================================
CREATE PROCEDURE [dbo].[InsertPrescription] 
@ExamID			INT = 0,
@IndividualID_Patient	INT,
@IndividualID_Doctor	INT,
@ODSphere		VARCHAR(10),
@OSSphere		VARCHAR(10),
@ODCylinder		VARCHAR(10),
@OSCylinder		VARCHAR(10),
@ODAxis			INT,
@OSAxis			INT,
@ODHPrism		DECIMAL(4,2),
@OSHPrism		DECIMAL(4,2),
@ODVPrism		DECIMAL(4,2),
@OSVPrism		DECIMAL(4,2),
@ODHBase		VARCHAR(3),
@OSHBase		VARCHAR(3),
@ODVBase		VARCHAR(4),
@OSVBase		VARCHAR(4),
@ODAdd			DECIMAL(4,2) = 0,
@OSAdd			DECIMAL(4,2) = 0,
@EnteredODSphere Decimal(4,2),
@EnteredOSSphere Decimal(4,2),
@EnteredODCylinder Decimal(4,2),
@EnteredOSCylinder Decimal(4,2),
@EnteredODAxis Int,
@EnteredOSAxis Int,
@ModifiedBy		VARCHAR(200),
@IsMonoCalculation		bit,
@PDDistant				Decimal(6,2),
@PDNear					Decimal(6,2),
@ODPDDistant			Decimal(4,2),
@ODPDNear				Decimal(4,2),
@OSPDDistant			Decimal(4,2),
@OSPDNear				Decimal(4,2),
@RxName			VARCHAR(20),
@ScannedRxID	INT = 0,
@Success		INT OUTPUT,
@RxID			INT OUTPUT,
@PrescriptionDate DateTime
			
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE @TempID INT 
		
	BEGIN TRY
		BEGIN TRANSACTION
			
			IF(@ExamID IS NULL )
			BEGIN
				INSERT INTO dbo.Prescription
				(ExamID, IndividualID_Patient, IndividualID_Doctor, ODSphere, OSSphere, ODCylinder, OSCylinder, ODAxis, OSAxis, ODHPrism, OSHPrism,
				ODVPrism, OSVPrism, ODHBase, OSHBase, ODVBase, OSVBase, ODAdd, OSAdd, PrescriptionDate, IsActive, EnteredODSphere, EnteredOSSphere,
				EnteredODCylinder, EnteredOSCylinder, EnteredODAxis,EnteredOSAxis,ModifiedBy, DateLastModified, LegacyID, IsUsed, IsMonoCalculation,
				PDNear, PDDistant, ODPDNear, ODPDDistant, OSPDNear, OSPDDistant, RxName,RxScanID)
				VALUES(0, @IndividualID_Patient, @IndividualID_Doctor, @ODSphere, @OSSphere, @ODCylinder, @OSCylinder, @ODAxis, @OSAxis, @ODHPrism, @OSHPrism, 
				@ODVPrism, @OSVPrism, @ODHBase, @OSHBase, @ODVBase, @OSVBase, @ODAdd, @OSAdd, @PrescriptionDate, 1, @EnteredODSphere, @EnteredOSSphere, 
				@EnteredODCylinder, @EnteredOSCylinder, @EnteredODAxis, @EnteredOSAxis, @ModifiedBy, GETDATE(), 0, 0,@IsMonoCalculation, @PDNear,
				@PDDistant, @ODPDNear, @ODPDDistant, @OSPDNear, @OSPDDistant,@RxName,@ScannedRxID)
			END
			ELSE
			begin
				INSERT INTO dbo.Prescription
				(ExamID, IndividualID_Patient, IndividualID_Doctor, ODSphere, OSSphere, ODCylinder, OSCylinder, ODAxis, OSAxis, ODHPrism, OSHPrism,
				ODVPrism, OSVPrism, ODHBase, OSHBase, ODVBase, OSVBase, ODAdd, OSAdd, PrescriptionDate, IsActive, EnteredODSphere, EnteredOSSphere,
				EnteredODCylinder, EnteredOSCylinder, EnteredODAxis,EnteredOSAxis,ModifiedBy, DateLastModified, LegacyID, IsUsed, IsMonoCalculation,
				PDNear, PDDistant, ODPDNear, ODPDDistant, OSPDNear, OSPDDistant,RxName,RxScanID)
				VALUES(@ExamID, @IndividualID_Patient, @IndividualID_Doctor, @ODSphere, @OSSphere, @ODCylinder, @OSCylinder, @ODAxis, @OSAxis, @ODHPrism, @OSHPrism, 
				@ODVPrism, @OSVPrism, @ODHBase, @OSHBase, @ODVBase, @OSVBase, @ODAdd, @OSAdd, @PrescriptionDate, 1, @EnteredODSphere, @EnteredOSSphere, 
				@EnteredODCylinder, @EnteredOSCylinder, @EnteredODAxis, @EnteredOSAxis, @ModifiedBy, GETDATE(), 0, 0,@IsMonoCalculation, @PDNear,
				@PDDistant,@ODPDNear,  @ODPDDistant, @OSPDNear, @OSPDDistant,@RxName,@ScannedRxID)
			END			
			SELECT @TempID = @@IDENTITY
			-- Insert Succeeded
			SET @Success = 1
			
			SET @RxID = @TempID
			
			----Auditing Routines Added 10/8/14 - BAF
			Declare @DateLastModified DateTime = GetDate() 
			Declare @RecID VarChar(50) = Cast(@TempID as VarChar(50))
			Declare @IsActive VarChar(1) = '1'
			Declare @RXInfo VarChar(Max) = (Cast(@ExamID as VarChar(15)) + ', ' + Cast(@IndividualID_Patient as VarChar(15)) + ', ' + Cast(@IndividualID_Doctor as VarChar(15))
				 + ', ' + Cast(@ODSphere as VarChar(7)) + ', ' + Cast(@OSSphere as VarChar(7)) + ', ' + Cast(@ODCylinder as VarChar(7)) + ', ' + 
				 Cast(@OSCylinder as VarChar(7)) + ', ' + Cast(@ODAxis as VarChar(7)) + ', ' + Cast(@OSAxis as VarChar(7)) + ', ' + Cast(@ODHPrism as VarChar(7))
				 + ', ' + Cast(@OSHPrism as VarChar(7)) + ', ' + Cast(@ODVPrism as VarChar(7)) + ', ' + Cast(@OSVPrism as VarChar(7)) + ', ' + 
				 @ODHBase + ', ' + @OSHBase + ', ' + @ODVBase + ', ' + @OSVBase + ', ' + Cast(@ODAdd as VarChar(7)) + ', ' + Cast(@OSAdd as VarChar(7)) +
				 ', ' + Cast(@PrescriptionDate as VarChar(20)) + ', ' +  @IsActive + ', ' + Cast(@EnteredODSphere as VarChar(7)) + ', ' + 
				 Cast(@EnteredOSSphere as VarChar(7)) + ', ' + Cast(@EnteredODCylinder as VarChar(7)) + ', ' + Cast(@EnteredOSCylinder as VarChar(7))
				  + ', ' + Cast(@EnteredODAxis as VarChar(7)) + ', ' + Cast(@EnteredOSAxis as VarChar(7)) + ', ' + @ModifiedBy + ', ' + Cast(@DateLastModified as VarChar(20))
				  + ', 0, 0, '+ CAST(@IsMonoCalculation AS CHAR(1)) + ', ' + CAST(@PDDistant AS VARCHAR(10)) + ', ' + CAST(@PDNear AS VARCHAR(10)) + ', ' +
				  CAST(@ODPDNear AS VARCHAR(10)) + ', ' + CAST(@ODPDNear AS VARCHAR(10)) + ', ' + CAST(@OSPDDistant AS VARCHAR(10)) + ', ' + 
				  CAST(@OSPDNear AS VARCHAR(10)) + ', ' + @RxName + ', ' + CAST(@ScannedRxID AS VARCHAR(10)))
				   
			IF @RecID is not null
				Exec InsertAuditTrail @DateLastModified,'ALL Prescription',@RXInfo,@RecID,11,@ModifiedBy,'I'
		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH
	

END
GO
/****** Object:  StoredProcedure [dbo].[GetRulesOfBehaviorDate]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 11 June 2019
-- Description:	Use to Retrieve the Rules of Behavior Date for a user
-- =============================================
CREATE PROCEDURE [dbo].[GetRulesOfBehaviorDate] 
	@UserName VarChar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Declare @ID int = 0

	Select @ID = IndividualID from dbo.Profile_PrimarySiteUnion
	where LowerUserName = @UserName

	If @ID > 0
		Begin
			Select RobAcceptance from dbo.Individual where ID = @ID
		End
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateIndividualPhoneNumberByID]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateIndividualPhoneNumberByID] 
@ID					INT,
@IndividualID		INT,
@PhoneNumberType	VARCHAR(5),
@AreaCode			VARCHAR(3),
@PhoneNumber		VARCHAR(15),
@Extension			VARCHAR(7),
@IsActive			BIT,
@ModifiedBy			VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	--Added for Auditing Purposes 10/10/14 - BAF
	Declare @PhoneRec as VarChar(50)
	
	Select @PhoneRec = COALESCE(@PhoneRec,'') + CAST(IndividualID as VarChar(20)) + ', ' +
		PhoneNumberType  + ', ' + AreaCode + ', ' + PhoneNumber  + ', ' + Extension
		 + ', ' + Cast(IsActive as CHAR(1))  + ', ' + ModifiedBy
	From dbo.IndividualPhoneNumber where ID = @ID
	----
		
	BEGIN TRY
		BEGIN TRANSACTION
		
			IF @IsActive = 1
				/* UPDATE THE PATIENT'S IsActive STATUS ON ALL ADDRESS RECORDS */				
				UPDATE IndividualPhoneNumber
				SET IsActive = 0
				WHERE IndividualID = @IndividualID
				
			UPDATE dbo.IndividualPhoneNumber
			SET AreaCode = @AreaCode, 
			DateLastModified = GETDATE(), 
			Extension = @Extension, 
			IndividualID = @IndividualID, 
			IsActive = @IsActive, 
			ModifiedBy = @ModifiedBy, 
			PhoneNumber = REPLACE(@PhoneNumber,'-',''),
			PhoneNumberType = @PhoneNumberType
			WHERE ID = @ID
				
			---- Auditing Insert -- 10/10/14 - BAF
			Declare @DateLastModified VarChar(20) = Cast(GetDate() as VarChar(20))
			
			EXEC InsertAuditTrail @DateLastModified, 'PhoneRecord',@PhoneRec, @ID,7, @ModifiedBy,'U'
			---

			SELECT AreaCode, 
			DateLastModified, 
			Extension, 
			ID, 
			IndividualID, 
			IsActive, 
			ModifiedBy, 
			PhoneNumber, 
			PhoneNumberType 
			FROM dbo.IndividualPhoneNumber
			WHERE IndividualID = @IndividualID	

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateIndividualIDNumberByID]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  10/21/15 - baf - to only update ID Numbers of the same type.
-- =============================================
CREATE PROCEDURE [dbo].[UpdateIndividualIDNumberByID]
@ID					INT,
@IndividualID		int,
@IDNumber			varchar(15),
@IDNumberType		varchar(3),
@IsActive			bit = true,
@ModifiedBy			varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TempID INT
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
		
	BEGIN TRY
		BEGIN TRANSACTION
			IF @IsActive = 1
						
			-- AUditing Routine - 10/10/14 - BAF
			Declare @IDRec varchar(20)
			
			--Used to ensure that FSN's are stored as SSNs
			IF (Len(@IDNumber) = '9') and (Left(@IDNumber,1) = '9') and (@IDNumberType <> 'SSN')
			Begin
				Set @IDNumberType = 'SSN'
			End
			
			Select @IDRec = COALESCE(@IDRec,'') + CAST(IndividualID as VarChar(20)) + ', ' +
				IDNumber  + ', ' + IDNumberType + ', ' + Cast(IsActive as CHAR(1))  + ', ' + ModifiedBy
			From dbo.IndividualIDNumber where ID = @ID
			------
			BEGIN
				UPDATE dbo.IndividualIDNumber
				SET IsActive = 0
				WHERE IndividualID = @IndividualID and IDNumberType = @IDNumberType;
			END

			UPDATE dbo.IndividualIDNumber
			SET IndividualID = @IndividualID, 
			IDNumber = @IDNumber, 
			IDNumberType = @IDNumberType, 
			IsActive = @IsActive, 
			ModifiedBy = @ModifiedBy, 
			DateLastModified = GETDATE()
			WHERE ID = @ID
			
			-- AUditing Routine - 10/10/14 - BAF
			Declare @RecDate VarChar(20) = Cast(GetDate() as VarChar(20))
			Declare @RecID VarChar(20) = Cast(@ID as VarChar(20))
			
			Exec InsertAuditTrail @RecDate, 'ID Number', @IDRec, @RecID, 21,@ModifiedBy, 'U'
			---- 
		COMMIT TRANSACTION

			SELECT ID, 
			IndividualID, 
			IDNumber, 
			IDNumberType, 
			IsActive, 
			ModifiedBy, 
			DateLastModified 
			FROM dbo.IndividualIDNumber
			WHERE IndividualID = @IndividualID
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[UpdateIndividualEMailAddressByID]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateIndividualEMailAddressByID] 
@ID				INT,
@IndividualID	INT,
@EMailType		VARCHAR(10),
@EMailAddress	VARCHAR(200),
@IsActive		BIT,
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	BEGIN TRY
		BEGIN TRANSACTION
		
			-- Auditing Routine -- 10/10/14 - BAF
			Declare @EmailRec VarChar(250)
			
			Select @EmailRec = COALESCE(@EmailRec,'') + CAST(IndividualID as VarChar(20)) + ', ' +
				EmailType  + ', ' + EmailAddress + ', ' +  Cast(IsActive as CHAR(1))  + ', ' + ModifiedBy
			From dbo.IndividualEmailAddress where ID = @ID
			-------------
			
			IF @IsActive = 1
				BEGIN
					/* UPDATE THE PATIENT'S IsActive STATUS ON ALL ADDRESS RECORDS */
					UPDATE IndividualEMailAddress
					SET IsActive = 0
					WHERE IndividualID = @IndividualID
				END
			
			UPDATE dbo.IndividualEMailAddress
			SET EMailType = @EMailType,
			IndividualID = @IndividualID,
			EMailAddress = @EMailAddress,
			IsActive = @IsActive,
			ModifiedBy = @ModifiedBy,
			DateLastModified = GETDATE()
			WHERE ID = @ID
								
			---- Auditing Insert -- 10/10/14 - BAF
			Declare @DateLastModified VarChar(20) = Cast(GetDate() as VarChar(20))
			
			EXEC InsertAuditTrail @DateLastModified, 'Email Record',@EmailRec, @ID,9, @ModifiedBy,'U'
			---
		
		COMMIT TRANSACTION
			SELECT DateLastModified, 
			EMailAddress, 
			EMailType, 
			ID, 
			IndividualID, 
			IsActive, 
			ModifiedBy 
			FROM dbo.IndividualEMailAddress
			WHERE IndividualID = @IndividualID
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateIndividualByID]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateIndividualByID] 
@ID						int,
--@PersonalType			varchar(25),
@FirstName				varchar(75),
@LastName				varchar(75),
@MiddleName				varchar(75),
@DateOfBirth			smalldatetime,
@EADStopDate			datetime = '01/01/1900',
@IsPOC					bit,
@SiteCodeID				varchar(6),
@Comments				varchar(256),
@IsActive				bit,
@LocationCode			varchar(5),
@Demographic			VARCHAR(8),
@ModifiedBy				varchar(200)
--@Success			INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int,
		@ROBAcceptance DateTime = Null

	SELECT @ErrorLogID = 0
	
	Select @ROBAcceptance = ROBAcceptance from dbo.Individual where ID = @ID

	--- Get the TypeID for the Personal Type
	--DECLARE @TypeID INT
	--SELECT @TypeID = ID FROM dbo.LookupTable
	--WHERE Value = @PersonalType

	BEGIN TRY
		--IF(LOWER(@PersonalType) = 'provider' OR LOWER(@PersonalType) = 'technician')
		--BEGIN
		--	DECLARE @tmpID int
		--	SELECT @TmpID = IndividualID FROM dbo.IndividualIDSiteCodeUnion
		--	WHERE IndividualID = @ID AND SiteCode = @SiteCodeID
		
		--	IF(@TmpID IS NULL)
		--	BEGIN
		--		INSERT INTO dbo.IndividualIDSiteCodeUnion (IndividualID, SiteCode)
		--		VALUES (@ID, @SiteCodeID)
		--	END
		--END
		
		BEGIN TRANSACTION
		
			-- Auditing Routine - 10/10/14 = BAF
			Declare @IndivRec as VarChar(775)
						
			--Select @IndivRec = COALESCE(@IndivRec,'') + @PersonalType + ', ' + FirstName + ', ' +
			Select @IndivRec = COALESCE(@IndivRec,'') + ', ' + FirstName + ', ' +
				MiddleName  + ', ' + LastName + ', ' +  CAST(DateOfBirth as VarChar(20)) + ', ' +
				Demographic + ', ' + CAST(EADStopDate as VarChar(20)) + ', ' + CAST(IsPOC as CHAR(1)) 
				+ ', ' + SiteCodeID + ', ' +  Comments + ', ' +  CAST(IsActive as CHAR(1)) + ', ' + 
				TheaterLocationCode + ', ' +  ModifiedBy
			From dbo.Individual where ID = @ID	
			---------
		
			UPDATE dbo.Individual
			SET
				--PersonalType = @PersonalType,
				FirstName = @FirstName, 
				LastName = @LastName, 
				MiddleName = @MiddleName, 
				DateOfBirth = @DateOfBirth,
				Demographic = @Demographic,
				EADStopDate = @EADStopDate,
				IsPOC = @IsPOC,
				SiteCodeID = @SiteCodeID,
				Comments = @Comments,
				IsActive = @IsActive,
				TheaterLocationCode = @LocationCode,
				ModifiedBy = @ModifiedBy,
				DateLastModified = GETDATE(),
				ROBAcceptance = @ROBAcceptance
				WHERE ID = @ID
			
			-- Update Succeeded
			--SET @Success = 1	

			--After Updating the Individual Table, add the new type id if Necessary
			DECLARE @RecCnt INT = 0;
			
			--SELECT @RecCnt = COUNT(*) FROM dbo.IndividualType WHERE IndividualID = @ID AND TypeID = @TypeID
			
			--IF @RecCnt = 0
			--BEGIN
			--	INSERT INTO dbo.IndividualType (IndividualID, TypeID ,IsActive ,ModifiedBy ,DateLastModified)
			--	VALUES  (@ID , @TypeID , 1, @ModifiedBy, GETDATE() )
			--END
			
			SET @RecCnt = 0
			Select @RecCnt = COUNT(*) from dbo.IndividualIDSiteCodeUnion 
			where IndividualID = @ID and SiteCode = @SiteCodeID
				
			If @RecCnt = 0
			Begin
				Insert into dbo.IndividualIDSiteCodeUnion(IndividualID, SiteCode)
				Values (@ID, @SiteCodeID)
			END
			
			-- Auditing Routine - 10/10/14 - BAF
			Declare @RecDate VarChar(20) = Cast(GetDate() as VarChar(20))
			
			EXEC InsertAuditTrail @RecDate, 'Individual Record',@IndivRec, @ID,5, @ModifiedBy,'U'
			----
				
		COMMIT TRANSACTION
		
				SELECT a.Comments, 
				a.DateLastModified, 
				a.DateOfBirth, 
				a.Demographic, 
				a.EADStopDate, 
				a.FirstName, 
				a.ID, 
				a.IsActive, 
				a.IsPOC, 
				a.LastName, 
				a.TheaterLocationCode, 
				a.MiddleName, 
				a.ModifiedBy, 
				lt.value as  PersonalType, 
				a.SiteCodeID,
				b.IDNumber,
				b.IDNumberType
				FROM dbo.Individual a
				INNER JOIN dbo.IndividualIDNumber b ON a.ID = b.IndividualID
				INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID
				INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.ID
				WHERE a.ID = @ID

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END

	   -- Insert Failed
	 -- Set @Success = 0 

	RETURN	
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateIndividualAddressByID]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Shear	
-- Create date: 20 Oct 2011
-- Description:	Update the address with uniquie key ID to new values passed in
-- =============================================
CREATE PROCEDURE [dbo].[UpdateIndividualAddressByID] 
@ID				int,
@IndividualID	int,
@Address1		varchar(200),
@Address2		varchar(200) = null,
@Address3		varchar(200) = null,
@City			varchar(200),
@State			varchar(2),
@Country		varchar(2),
@ZipCode		varchar(10),
@AddressType	varchar(10),
@IsActive		bit = 1,
@ModifiedBy		varchar(200),
@ExpireDays		INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	BEGIN TRY
		BEGIN TRANSACTION
		
			-- Auditing Routine 10/10/14 - BAF
			Declare @AddrRec VarChar(1050)
			Declare @RecDate VarChar(20) = Cast(GetDate() as VarChar(20))
			
			Select @AddrRec = COALESCE(@AddrRec,'') + CAST(IndividualID as VarChar(200)) + ', ' + Address1 + ', ' +
				Address2 + ', ' + Address3 + ', ' + City + ', ' + [State] + ', ' + Country + ', ' + ZipCode + ', ' +
				AddressType + ', ' + CAST(IsActive as CHAR(1)) + ', ' + ModifiedBy + ', ' + UIC
			From dbo.IndividualAddress where ID = @ID
			
		
			EXEC InsertAuditTrail @RecDate, 'Address Record',@AddrRec, @ID,6, @ModifiedBy,'U'
			---
			
			UPDATE dbo.IndividualAddress
			SET
			IndividualID = @IndividualID,
			Address1 = @Address1,
			Address2 = @Address2,
			Address3 = @Address3,
			City = @City,
			[State] = @State,
			Country = @Country,
			ZipCode = @ZipCode,
			AddressType = @AddressType,
			IsActive = @IsActive,
			ModifiedBy = @ModifiedBy,
			DateLastModified = GETDATE(),
			DateVerified = GETDATE(),
			VerifiedBy = @ModifiedBy,
			ExpireDays = @ExpireDays
			WHERE ID = @ID
		COMMIT TRANSACTION
			SELECT ID, 
			IndividualID, 
			Address1, 
			Address2, 
			Address3, 
			City, 
			[State], 
			Country, 
			ZipCode, 
			AddressType, 
			IsActive, 
			ModifiedBy, 
			DateLastModified,
			DateVerified,
			VerifiedBy,
			ExpireDays
			FROM dbo.IndividualAddress
			WHERE IndividualID = @IndividualID
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateExamByExamID]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Shear
-- Create date: 17 Oct 2011
-- Description:	Update exam columns by unique Exam ID
-- =============================================
CREATE PROCEDURE [dbo].[UpdateExamByExamID]
@ID							int, 
@IndividualID_Patient		int,
--@AddressID_Patient			INT,
@IndividualID_Examiner		int,
@ODCorrectedAcuity			VARCHAR(12),
@ODUncorrectedAcuity		VARCHAR(12),
@OSCorrectedAcuity			VARCHAR(12),
@OSUncorrectedAcuity		VARCHAR(12),
@ODOSCorrectedAcuity		VARCHAR(12),
@ODOSUncorrectedAcuity		VARCHAR(12),
@ExamDate					DATETIME,
@Comment					VARCHAR(256),
@IsActive					BIT,
@ModifiedBy					VARCHAR(200),
@Success					INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
		
	BEGIN TRY
		BEGIN TRANSACTION
		
			-- Auditing Routine - 10/10/14 BAF
			Declare @ExamRec as VarChar(615)
			
			Select @ExamRec = COALESCE(@ExamRec,'') + CAST(IndividualID_Patient as VarChar(20)) + ', ' +
				CAST(IndividualID_Examiner as VarChar(20)) + ', ' +
				ODCorrectedAcuity + ', ' + ODUncorrectedAcuity + ', ' + OSCorrectedAcuity + ', ' +
				OSUncorrectedAcuity + ', ' + ODOSCorrectedAcuity + ', ' + ODOSUncorrectedAcuity + ', ' +
				Comment + ', ' + CAST(IsActive as CHAR(1)) + ', ' + CAST(ExamDate as VarChar(20)) + ', ' +
				ModifiedBy + CAST(DateLastModified as VarChar(20))
			From dbo.Exam where ID = @ID
			-----------
			
			UPDATE dbo.Exam
			SET IndividualID_Patient = @IndividualID_Patient,
			--AddressID_Patient = @AddressID_Patient,
			IndividualID_Examiner = @IndividualID_Examiner,
			ODCorrectedAcuity = @ODCorrectedAcuity,
			ODUncorrectedAcuity = @ODUncorrectedAcuity,
			OSCorrectedAcuity = @OSCorrectedAcuity,
			OSUncorrectedAcuity = @OSUncorrectedAcuity,
			ODOSCorrectedAcuity = @ODOSCorrectedAcuity,
			ODOSUncorrectedAcuity = @ODOSUncorrectedAcuity,
			Comment = @Comment,
			ExamDate = @ExamDate,
			IsActive = @IsActive,
			ModifiedBy = @ModifiedBy,
			DateLastModified = GETDATE()
			WHERE ID = @ID
			
			-- Auditing Routine -- 10/10/14 - BAF
			Declare @RecDate VarChar(20) = Cast(GetDate() as VarChar(20))
			
			Exec InsertAuditTrail @RecDate, 'Exam Record', @ExamRec, @ID, 10,@ModifiedBy,'U'
			-----------------

		COMMIT TRANSACTION
		--SELECT 	ID, 
		--IndividualID_Patient,
		----AddressID_Patient,
		--IndividualID_Examiner,
		--ODCorrectedAcuity,
		--ODUncorrectedAcuity,
		--OSCorrectedAcuity,
		--OSUncorrectedAcuity,
		--ODOSCorrectedAcuity,
		--ODOSUncorrectedAcuity,
		--Comment,
		--ExamDate,
		--ModifiedBy,
		--IsActive,
		--DateLastModified
		--FROM dbo.Exam
		--WHERE ID = @ID
		
		-- Update Succeeded
		SET @Success = 1
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			SET @Success = 0 -- Update Failed
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
	END CATCH
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateAuthorizationSSOByUserName]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateAuthorizationSSOByUserName]
(
	@UserName nvarchar(256),
	@SSO_UserName nvarchar(256)
	
)
AS
	SET NOCOUNT OFF;

--DECLARE @UserName nvarchar(256)
--DECLARE @SSO_UserName nvarchar(256)
--SET @UserName = 'clinicadmin'
--SET @SSO_UserName = 'labtech'
BEGIN

	IF NOT EXISTS
	(
		SELECT UserName
		FROM [dbo].[Authorization]
		WHERE UserName = @SSO_UserName	
	)
	BEGIN
			EXEC InsertAuthorizationByUserName @SSO_UserName
	END
	
	--IF NOT ADMIN ACCOUNT
	IF NOT EXISTS
	(

		SELECT Distinct a.UserName
		FROM [dbo].[aspnet_Users] a
		left outer join [dbo].[aspnet_UsersInRoles] ri on a.UserId=ri.UserId
		left outer join aspnet_Roles r on ri.RoleId=r.RoleId
		WHERE a.UserName = @UserName 
		and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin')		
		--and SUBSTRING(r.RoleName , len(r.RoleName )-4, 5) = 'admin'
		UNION
		SELECT Distinct a.UserName
		FROM [dbo].[aspnet_Users] a
		left outer join [dbo].[aspnet_UsersInRoles] ri on a.UserId=ri.UserId
		left outer join aspnet_Roles r on ri.RoleId=r.RoleId
		WHERE a.UserName = @SSO_UserName 
		and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin')		
		--and SUBSTRING(r.RoleName , len(r.RoleName )-4, 5) = 'admin'	
	)
	BEGIN
		IF Exists
		( 
			--Check if the second user account has an SSO_UserName
			SELECT [dbo].[Authorization].SSO_UserName
			FROM [dbo].[Authorization]
			WHERE [dbo].[Authorization].UserName = @SSO_UserName 
			  and [dbo].[Authorization].SSO_UserName is not null 
			  and  [dbo].[Authorization].SSO_UserName!=''
		)
		BEGIN
				--Use the second user account's SSO_UserName	
				UPDATE [dbo].[Authorization]
				SET SSO_UserName = (SELECT [dbo].[Authorization].SSO_UserName
									FROM [dbo].[Authorization]
									WHERE [dbo].[Authorization].UserName = @SSO_UserName)
				WHERE UserName=@UserName
		END
		ElSE
		BEGIN
				--Update the User Account with the name of the second user account
				UPDATE [dbo].[Authorization]
				SET SSO_UserName = @SSO_UserName
				WHERE UserName=@UserName
				
				--Update the second user account with the SSO
				UPDATE [dbo].[Authorization]
				SET SSO_UserName = @SSO_UserName
				WHERE UserName=@SSO_UserName
		END
	END
ELSE 
BEGIN
	--IF @UserName is an admin Account 
		IF EXISTS
		(
			SELECT Distinct a.UserName
			FROM [dbo].[aspnet_Users] a
			left outer join [dbo].[aspnet_UsersInRoles] ri on a.UserId=ri.UserId
			left outer join aspnet_Roles r on ri.RoleId=r.RoleId
			WHERE a.UserName = @UserName 
			and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin')			
			--and SUBSTRING(r.RoleName , len(r.RoleName )-4, 5) = 'admin'
		)
		BEGIN
			--IF @SSO_UserName is an admin Account 		
			IF EXISTS
			(
				SELECT Distinct a.UserName
				FROM [dbo].[aspnet_Users] a
				left outer join [dbo].[aspnet_UsersInRoles] ri on a.UserId=ri.UserId
				left outer join aspnet_Roles r on ri.RoleId=r.RoleId
				WHERE a.UserName = @SSO_UserName 
				and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin')				
				--and SUBSTRING(r.RoleName , len(r.RoleName )-4, 5) = 'admin'	
			)
			BEGIN
				IF Exists
				( 
					--Check if the second user account has an SSO_UserName
					SELECT [dbo].[Authorization].SSO_UserName
					FROM [dbo].[Authorization]
					WHERE [dbo].[Authorization].UserName = @SSO_UserName 
					  and [dbo].[Authorization].SSO_UserName is not null 
					  and  [dbo].[Authorization].SSO_UserName!=''
				)
				BEGIN
					--Use the second user account's SSO_UserName	
					UPDATE [dbo].[Authorization]
					SET SSO_UserName = (SELECT [dbo].[Authorization].SSO_UserName
										FROM [dbo].[Authorization]
										WHERE [dbo].[Authorization].UserName = @SSO_UserName)
					WHERE UserName=@UserName
				END
				ElSE
				BEGIN
					--Update the User Account with the name of the second user account
					UPDATE [dbo].[Authorization]
					SET SSO_UserName = @SSO_UserName
					WHERE UserName=@UserName
					
					--Update the second user account with the SSO
					UPDATE [dbo].[Authorization]
					SET SSO_UserName = @SSO_UserName
					WHERE UserName=@SSO_UserName
				END
			END
			ELSE
			BEGIN
				--CHECK @SSO_UserName
				IF NOT Exists
				( 
					--Check if the second user account has an SSO_UserName
					SELECT [dbo].[Authorization].SSO_UserName
					FROM [dbo].[Authorization]
					WHERE [dbo].[Authorization].UserName = @UserName 
					  and [dbo].[Authorization].SSO_UserName is not null 
					  and  [dbo].[Authorization].SSO_UserName!=''
				)
				BEGIN
						--Use the second user account's SSO_UserName	
						UPDATE [dbo].[Authorization]
						SET SSO_UserName = @UserName
						WHERE UserName=@UserName
				END				
			END
		END
		ELSE
		BEGIN
			--CHECK SSO_UserName
			IF NOT Exists
			( 
				--Check if the second user account has an SSO_UserName
				SELECT [dbo].[Authorization].SSO_UserName
				FROM [dbo].[Authorization]
				WHERE [dbo].[Authorization].UserName = @UserName 
				  and [dbo].[Authorization].SSO_UserName is not null 
				  and  [dbo].[Authorization].SSO_UserName!=''
			)
			BEGIN
					--Use the second user account's SSO_UserName	
					UPDATE [dbo].[Authorization]
					SET SSO_UserName = @UserName
					WHERE UserName=@UserName
			END	
		END

	END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateAuthorizationCacInfoByUserName]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[UpdateAuthorizationCacInfoByUserName]

	@CAC_ID nvarchar(256),
	@issuerName nvarchar(256),
	@UserName nvarchar(256)

AS

	SET NOCOUNT ON;


	--DECLARE @CAC_ID nvarchar(256)
	--DECLARE @issuerName nvarchar(256)
	--DECLARE @UserName nvarchar(256)
	
	--SET @CAC_ID = '1014240310'
	--SET @issuerName = 'DOD EMAIL CA-31'
	--SET @UserName = 'carlosct'	

	IF NOT EXISTS
	(
		SELECT UserName
		FROM [dbo].[Authorization]
		WHERE UserName = @UserName	
	)
	BEGIN
			EXEC InsertAuthorizationByUserName @UserName
	END


	BEGIN
		--CHECK IF ADMIN ACCOUNT
		IF EXISTS
		(
			SELECT Distinct a.UserName
			FROM [dbo].[Authorization] a
			left outer join [dbo].[aspnet_Users] u on a.UserName = u.UserName
			left outer join [dbo].[aspnet_UsersInRoles] ri on u.UserId=ri.UserId
			left outer join aspnet_Roles r on ri.RoleId=r.RoleId
			WHERE a.UserName = @UserName and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin')			
		)
		BEGIN
			--CHECK IF ANOTHER ACCOUNT USES CAC ID AND MAKE SURE IS ALSO AN ADMIN
			IF EXISTS
			(
				SELECT Distinct a.UserName
				FROM [dbo].[Authorization] a
				left outer join [dbo].[aspnet_Users] u on a.UserName = u.UserName
				left outer join [dbo].[aspnet_UsersInRoles] ri on u.UserId=ri.UserId
				left outer join aspnet_Roles r on ri.RoleId=r.RoleId		
				WHERE (a.CacId = @CAC_ID and a.CacIssuer = 'DOD' --@issuerName 
					and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin')) OR a.CacId is null OR a.CacId = ''
			)
			BEGIN
				
				If ISNUMERIC(@Cac_ID) = 1
				BEGIN
					UPDATE [dbo].[Authorization]
					SET CacId = @CAC_ID, CacIssuer = @issuerName
					WHERE UserName = @UserName 
				END
			END		
		END
		ELSE
		BEGIN
				--CHECK IF ANOTHER ACCOUNT USES CAC ID ADN MAKE SURE IS NOT AN ADMIN
				IF NOT EXISTS
				(
					SELECT Distinct a.UserName
					FROM [dbo].[Authorization] a
					left outer join [dbo].[aspnet_Users] u on a.UserName = u.UserName
					left outer join [dbo].[aspnet_UsersInRoles] ri on u.UserId=ri.UserId
					left outer join aspnet_Roles r on ri.RoleId=r.RoleId		
					WHERE a.CacId = @CAC_ID and a.CacIssuer = 'DOD EMAIL'--@issuerName 
						and LOWER(r.RoleName) in('labadmin', 'clinicadmin', 'humanadmin', 'mgmtenterprise', 'mgmtadmin') 
				)
				BEGIN		
					If ISNUMERIC(@Cac_ID) = 1
					Begin	
						UPDATE [dbo].[Authorization]
						SET CacId = @CAC_ID, CacIssuer = @issuerName
						WHERE UserName = @UserName 
					End				
				END
		END		
	END
GO
/****** Object:  StoredProcedure [dbo].[UpdateAddressByID]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Shear	
-- Create date: 20 Oct 2011
-- Description:	Update the address with uniquie key ID to new values passed in
-- ModifiedBy:  30 Oct 2017 - baf - Added Verification Info
-- =============================================
CREATE PROCEDURE [dbo].[UpdateAddressByID] 
@ID				int,
@IndividualID	int,
@Address1		varchar(200),
@Address2		varchar(200) = '',
@Address3		varchar(200) = '',
@City			varchar(200),
@State			varchar(2),
@Country		varchar(2),
@ZipCode		varchar(10),
@AddressType	varchar(10),
@IsActive		bit = 1,
@UIC			varchar(6) = '',
@ModifiedBy		varchar(200),
@ExpireDays		INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	BEGIN TRY
		BEGIN TRANSACTION
		
		-- Auditing Routine 10/10/14 - BAF
		Declare @AddrRec VarChar(1050)
		
		Select @AddrRec = COALESCE(@AddrRec,'') + CAST(IndividualID as VarChar(200)) + ', ' + Address1 + ', ' +
			Address2 + ', ' + Address3 + ', ' + City + ', ' + [State] + ', ' + Country + ', ' + ZipCode + ', ' +
			AddressType + ', ' + CAST(IsActive as CHAR(1)) + ', ' + ModifiedBy + ', ' + Cast(DateLastModified as VarChar(20))
			+ ', ' + UIC
		From dbo.IndividualAddress where ID = @ID
		
		-- Auditing Routine - 10/10/14 - BAF
		Declare @RecDate VarChar(20) = Cast(GetDate() as VarChar(20))
		Declare @RecID VarChar(20) = Cast(@ID as VarChar(20))
		
		Exec InsertAuditTrail @RecDate, 'ID Number', @AddrRec, @RecID, 21,@ModifiedBy, 'U'

		---
		
		IF @IsActive = 1
			BEGIN
				/* UPDATE THE PATIENT'S IsActive STATUS ON ALL ADDRESS RECORDS */
				UPDATE IndividualAddress
				SET IsActive = 0
				WHERE IndividualID = @IndividualID and ID <> @ID
			END
		
		IF(@UIC IS NULL)
			BEGIN
				UPDATE dbo.IndividualAddress
				SET
				IndividualID = @IndividualID,
				Address1 = @Address1,
				Address2 = @Address2,
				Address3 = @Address3,
				City = @City,
				[State] = @State,
				Country = @Country,
				ZipCode = @ZipCode,
				AddressType = @AddressType,
				IsActive = @IsActive,
				ModifiedBy = @ModifiedBy,
				DateLastModified = GETDATE(),
				DateVerified = GETDATE(),
				VerifiedBy = @ModifiedBy,
				ExpireDays = @ID
				WHERE ID = @ID and IsActive = 1
			END
		ELSE
			BEGIN
				UPDATE dbo.IndividualAddress
				SET
				IndividualID = @IndividualID,
				Address1 = @Address1,
				Address2 = @Address2,
				Address3 = @Address3,
				City = @City,
				[State] = @State,
				Country = @Country,
				ZipCode = @ZipCode,
				AddressType = @AddressType,
				UIC = @UIC,
				IsActive = @IsActive,
				ModifiedBy = @ModifiedBy,
				DateLastModified = GETDATE(),
				DateVerified = GETDATE(),
				VerifiedBy = @ModifiedBy,
				ExpireDays = @ExpireDays
				WHERE ID = @ID and IsActive = 1
			END
		
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
	-- Get a return record
	SELECT ID, 
	IndividualID, 
	Address1, 
	Address2, 
	Address3, 
	City, 
	[State], 
	Country, 
	ZipCode, 
	AddressType,
	UIC, 
	IsActive, 
	ModifiedBy, 
	DateLastModified,
	DateVerified,
	VerifiedBy,
	ExpireDays
	FROM dbo.IndividualAddress
	WHERE IndividualID = @IndividualID
  	and IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePrescriptionIsUsed]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9 July 2014
-- Description:	This stored procedure is used to update the
--		IsUsed bit when a Prescription is used in an order.
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePrescriptionIsUsed]
	-- Add the parameters for the stored procedure here
	@RxID Int,
	@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update dbo.Prescription set IsUsed = 1 where ID = @RxID;
	
	----Auditing -- BAF 10/9/14
	Declare @RecID VarChar(15) = Cast(@RxID as VarChar(15))
	Declare @DateLastModified DateTime = Cast(GetDate() as VarChar(20))
	Declare @ChangeInfo VarChar(Max) = '1'
		
	Exec InsertAuditTrail @DateLastModified, 'IsUsed',@ChangeInfo,@RecID,11,@ModifiedBy,'U'
	-------------------
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePrescription]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  16 Mar 2018 - baf - Added Scanned Rx ID
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePrescription] 
@ID				INT,
@ExamID			INT = NULL,
@IndividualID_Patient	INT,
@IndividualID_Doctor	INT,
@ODSphere		VARCHAR(10),
@OSSphere		VARCHAR(10),
@ODCylinder		VARCHAR(10),
@OSCylinder		VARCHAR(10),
@ODAxis			INT = NULL,
@OSAxis			INT = NULL,
@ODHPrism		DECIMAL(4,2),
@OSHPrism		DECIMAL(4,2),
@ODVPrism		DECIMAL(4,2),
@OSVPrism		DECIMAL(4,2),
@ODHBase		VARCHAR(3),
@OSHBase		VARCHAR(3),
@ODVBase		VARCHAR(4),
@OSVBase		VARCHAR(4),
@ODAdd			DECIMAL(4,2),
@OSAdd			DECIMAL(4,2),
@EnteredODSphere Decimal(4,2),
@EnteredOSSphere Decimal(4,2),
@EnteredODCylinder Decimal(4,2),
@EnteredOSCylinder Decimal(4,2),
@EnteredODAxis Int,
@EnteredOSAxis Int,
@IsActive		BIT,
@ModifiedBy		VARCHAR(200),
@IsMonoCalculation	BIT,
@ODPDDistant	DECIMAL(4,2),
@ODPDNear		DECIMAL(4,2),
@OSPDDistant	DECIMAL(4,2),
@OSPDNear		DECIMAL(4,2),
@PDDistant		DECIMAL(4,2),
@PDNear			DECIMAL(4,2),
@RxName			VARCHAR(20),
@ScannedRxID	INT,
@PrescriptionDate DateTime,
@Success		INT OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	If (Select IsUsed from dbo.Prescription where ID = @ID) = 0
	BEGIN TRY
	
			BEGIN TRANSACTION

			--Auditing Routine 10/10/14 BAF
			Declare @RxRec as VarChar(500)
			
			Select @RxRec = Coalesce(@RxRec,'Prescription: ') + CAST(ExamID as VarChar(20)) + ', ' + CAST(IndividualID_Patient as VarChar(20)) + ', '
				+ Cast(IndividualID_Doctor as VarChar(20)) + ', ' + Cast(ODSphere as VarChar(7)) + ', ' + Cast(OSSphere as VarChar(7)) 
				+ ', ' + Cast(ODCylinder as VarChar(7)) + ', ' + Cast(OSCylinder as VarChar(7)) + ', ' + Cast(ODAxis as VarChar(20)) 
				+ ', ' + Cast(OSAxis as VarChar(20)) + ', ' + Cast(ODHPrism as VarChar(7)) + ', '+ CAST(OSHPrism as VarChar(7)) + ', ' + 
				CAST(ODVPrism as VarChar(7)) + ', ' + CAST(OSVPrism as VarChar(7)) + ', ' +	ODHBase + ', ' + OSHBase + ', ' + 
				ODVBase + ', ' + OSVBase + ', ' + CAST(ODAdd as VarChar(7)) + ', ' + CAST(OSAdd as VarChar(7)) + ', ' +
				CAST(EnteredODSphere as VarChar(7)) + ', ' + CAST(EnteredOSSphere as VarChar(7)) + CAST(EnteredODCylinder as VarChar(7))
				+ ', ' + CAST(EnteredOSCylinder as VarChar(7)) + ', ' + Cast(EnteredODAxis as VarChar(7)) + ', ' + CAST(EnteredOSAxis as VarChar(7))
				+ ', ' + CAST(PrescriptionDate as VarChar(20)) + ', ' + CAST(IsUsed as VarChar(1)) + ', ' + CAST(IsActive as VarChar(1))
				+ ', ' + @ModifiedBy + ', ' + CAST(DateLastModified as VarChar(20)) + ', ' + CAST(ISMonocalculation AS CHAR(1)) + ', ' +
				CAST(ODPDDistant AS VARCHAR(7)) + ', ' + CAST(ODPDNear AS VARCHAR(7)) + ', ' + CAST(OSPDDistant AS VARCHAR(7)) + ', ' +
				CAST(OSPDNear AS VARCHAR(7)) + ', ' + CAST(PDDistant AS VARCHAR(7)) + ', ' + CAST(PDNear AS VARCHAR(7)) + ', ' + RxName +
				CAST(RxScanID AS VARCHAR(10))+ ', ' + Cast(@PrescriptionDate as VarChar(20))
			From dbo.Prescription where ID = @ID
			----
			
			UPDATE dbo.Prescription
			SET ExamID = @ExamID, 
			IndividualID_Patient = @IndividualID_Patient,
			IndividualID_Doctor = @IndividualID_Doctor, 
			ODSphere = @ODSphere, 
			OSSphere = @OSSphere, 
			ODCylinder = @ODCylinder, 
			OSCylinder = @OSCylinder, 
			ODAxis = @ODAxis, 
			OSAxis = @OSAxis, 
			ODHPrism = @ODHPrism, 
			OSHPrism = @OSHPrism,
			ODVPrism = @ODVPrism, 
			OSVPrism = @OSVPrism, 
			ODHBase = @ODHBase, 
			OSHBase = @OSHBase, 
			ODVBase = @ODVBase, 
			OSVBase = @OSVBase, 
			ODAdd = @ODAdd, 
			OSAdd = @OSAdd,
			EnteredODSphere = @EnteredODSphere,
			EnteredOSSphere = @EnteredOSSphere,
			EnteredODCylinder = @EnteredODCylinder,
			EnteredOSCylinder = @EnteredOSCylinder,
			EnteredODAxis = @EnteredODAxis,
			EnteredOSAxis = @EnteredOSAxis,
			IsActive = @IsActive, 
			ModifiedBy = @ModifiedBy, 
			DateLastModified = GETDATE(),
			IsMonoCalculation = @IsMonoCalculation,
			ODPDDistant = @ODPDDistant,
			ODPDNear = @ODPDNear,
			OSPDDistant = @OSPDDistant,
			OSPDNear = @OSPDNear,
			PDDistant = @PDDistant,
			PDNear = @PDNear,
			RxName = @RxName,
			RxScanID = @ScannedRxID,
			PrescriptionDate = @PrescriptionDate
			WHERE ID = @ID
			
			---Auditing Routine 10/10/14 BAF
			Declare @RecDate VarChar(20) = Cast(GetDate() as VarChar(20))
			
			Exec InsertAuditTrail @RecDate, 'Prescription Record', @RxRec, @ID, 11,@ModifiedBy,'U'
			------
		COMMIT TRANSACTION
		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
		SET @Success = 0
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
	Else
		SELECT 	ID,
				ExamID,
				IndividualID_Patient,
				IndividualID_Doctor,
				ODSphere,
				OSSphere,
				ODCylinder,
				OSCylinder,
				ODAxis,
				OSAxis,
				ODHPrism,
				OSHPrism,
				ODVPrism,
				OSVPrism,
				ODHBase,
				OSHBase,
				ODVBase,
				OSVBase,
				ODAdd,
				OSAdd,
				EnteredODSphere,
				EnteredOSSphere,
				EnteredODCylinder,
				EnteredOSCylinder,
				EnteredODAxis,
				EnteredOSAxis,
				PrescriptionDate,
				IsUsed,
				IsActive,
				ModifiedBy,
				DateLastModified,
				IsMonoCalculation,
				ODPDDistant,
				ODPDNear,
				OSPDDistant,
				OSPDNear,
				PDDistant,
				PDNear, 
				RxName,
				RxScanID,
				PrescriptionDate
		FROM dbo.Prescription
		WHERE ID = @ID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateLoggedInSite]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 17 Nov 2016
-- Description: This sp will update the users
--     Logged In SiteCode in the table Profile_PrimarySiteUnion
-- =============================================
CREATE PROCEDURE [dbo].[UpdateLoggedInSite]
	-- Add the parameters for the stored procedure here
	@LoweredUserName VARCHAR(30),
	@SiteCode VARCHAR(6)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Profile_PrimarySiteUnion SET LoggedInSiteCode = @SiteCode
	WHERE LowerUserName = @LoweredUserName
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateMedProsDispenseFlag]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 18 Apr 14
-- Description:	Update the MedProsDispensed flag to 1 (true) after MedPros 
--				records have beed sent to then via a call to the 
--				GetPatientOrdersForMedProsOrders stored procedure in code.
-- =============================================
CREATE PROCEDURE [dbo].[UpdateMedProsDispenseFlag]
	@OrderNumber varchar(16)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	UPDATE PatientOrder
	SET MedProsDispense = 1
	WHERE OrderNumber = @OrderNumber;
	
	--- Auditing Routines Added - 10/9/14  BAF
	Declare @DateLastModified VarChar(20) = Cast(GetDate() as VarChar(20))
	Declare @Flag Char(1) = '0'
	
	Exec InsertAuditTrail @DateLastModified, 'MedProsDispense',@Flag,@OrderNumber,12,'WebService','U'
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateIndividualType]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 7 November 2014
-- Description:	This stored procedure will update the
--	 IndividualType record by resetting the IsActive field
-- =============================================
CREATE PROCEDURE [dbo].[UpdateIndividualType]
	-- Add the parameters for the stored procedure here
	@IndividualTypeID	INT,
	@IsActive		BIT,
	@ModifiedBy		VARCHAR(256)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT
	SELECT @ErrorLogID = 0

    -- Insert statements for procedure here
	BEGIN TRY
			BEGIN TRANSACTION
				UPDATE dbo.IndividualType 
				SET IsActive = @IsActive,
				ModifiedBy = @ModifiedBy,
				DateLastModified = GETDATE()
				WHERE ID = @IndividualTypeID
			COMMIT TRANSACTION
			
			DECLARE @IndividualId	INT
			SELECT @IndividualId = i.IndividualID FROM IndividualType i WHERE i.ID = @IndividualTypeID
			
			EXEC dbo.GetIndividualTypesByIndividualID @IndividualId
	END TRY
	BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePimrsDispenseFlag]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 18 Apr 14
-- Description:	Update the PimrsDispensed flag to 1 (true) after PIMRS 
--				records have beed sent to then via a call to the 
--				GetPatientOrdersForPIMRSSinceLast stored procedure in code.
-- MOdified:  17 May 16 - baf - MOdified to take a list of order numbers from PIMRS and
--				Update the flag to 3 - ASIMS received final record of order
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePimrsDispenseFlag]
	@OrdersIn VARCHAR(MAX) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @OrderNum VARCHAR(MAX),
		@RecCnt INT,
		@CurRec INT = 1,
		@Order VARCHAR(16)
		
	CREATE TABLE #tmpOrderNbr
	(RowID INT, OrderNumber VARCHAR(16))
	
	IF @OrdersIn IS NOT NULL
	BEGIN
		IF RIGHT(@OrdersIn,1) = ','
		BEGIN
			SET @OrderNum = LEFT(@OrdersIn, LEN(@OrdersIn) -1)
			
			INSERT INTO #tmpOrderNbr
			SELECT * FROM dbo.SplitStrings(@OrderNum,',')
		END
		ELSE IF @OrdersIn IS NOT NULL
		BEGIN
			SET @OrderNum = @OrdersIn
			
			INSERT INTO #tmpOrderNbr
			SELECT * FROM dbo.SplitStrings(@OrderNum,',')
		END
		SELECT @RecCnt = COUNT(OrderNumber) FROM #tmpOrderNbr
		
		DECLARE CurOrders CURSOR
		FOR
			SELECT OrderNumber FROM #tmpOrderNbr
			
			DECLARE @ID VARCHAR(16)
			OPEN CurOrders
			FETCH Next FROM CurOrders INTO @ID
			WHILE (@@FETCH_STATUS <> -1)
			BEGIN
				UPDATE dbo.PatientOrder SET PimrsDispense = 3 WHERE OrderNumber = @ID AND PimrsDispense = 2
				FETCH Next FROM CurOrders INTO @ID
			END		
		CLOSE CurOrders
		DEALLOCATE CurOrders
	END
	DROP TABLE #tmpOrderNbr
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateScannedRx]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 March 2018
-- Description:	Used to update a ScannedRx record
-- =============================================
CREATE PROCEDURE [dbo].[UpdateScannedRx]
	-- Add the parameters for the stored procedure here
	@ID int,
	@RxID int,
	@IndividualID int,
	@DocName VarChar(500),
	@DocType VarChar(50),
	@RxScan VarBinary(Max),
	@ScanDate DateTime,
	@DelDate DateTIme,
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	BEGIN TRY
		BEGIN TRANSACTION
			Update dbo.ScannedRx Set IndividualID = @IndividualID, RxID = @RxID,
				DocName= @DocName, DocType = @DocType, RxScan = @RxScan,
				ScanDate = GETDATE(), DelDate = DATEADD(dd,91,GetDate())
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePatientOrder_BarCode]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 17 Dec 2013
-- Description:	This stored procedure adds the 
--		ONBarcode to the Patient Order
-- Modified:  1/3/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePatientOrder_BarCode]
	@OrderNumber VarChar(16),
	@BarCode VarBinary(Max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update dbo.PatientOrder set ONBarCode = @BarCode
	where OrderNumber = @OrderNumber;
	
	SELECT OrderNumber, IndividualID_Patient, IndividualID_Tech, PatientPhoneID, Demographic, LensType, LensMaterial, 
	Tint, Coatings, ODSegHeight, OSSegHeight, NumberOfCases, Pairs, PrescriptionID, FrameCode, FrameColor, FrameEyeSize, FrameBridgeSize, 
	FrameTempleType, ClinicSiteCode, LabSiteCode, ShipToPatient, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity, ShipState, 
	ShipZipCode, ShipCountry, ShipAddressType, LocationCode, UserComment1, UserComment2, IsGEyes, IsMultivision, IsActive, 
	PatientEmail, MedProsDispense, PimrsDispense, OnholdForConfirmation, VerifiedBy, ModifiedBy, DateLastModified, ONBarCode
	FROM dbo.PatientOrder
	WHERE OrderNumber = @OrderNumber 

END
GO
/****** Object:  StoredProcedure [dbo].[UnlinkOrders]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: Jan 6, 2016
-- Description:	This stored procedure resets the LinkedID field in a
--		PatientOrder to Null
-- =============================================
CREATE PROCEDURE [dbo].[UnlinkOrders]
	-- Add the parameters for the stored procedure here
	@OrderNumber		VARCHAR(MAX)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 DECLARE @ErrorLogID	INT = 0,
		@RecCnt INT,
		@Order VARCHAR(16),
		@CurRec INT = 1,
		@LinkedID VarChar(16);

	BEGIN TRY
	
		-- Need to loop through the order numbers if there is more than one
		-- Added 3/31/15 -- Check-In/Check-Out Changes

		CREATE TABLE #tmpOrderNbrs
			(RowID int, OrderNbr VarChar(16))
			
		INSERT INTO #tmpOrderNbrs
		SELECT * FROM dbo.SplitStrings(@OrderNumber,',')	

		SELECT * FROM #tmpOrderNbrs;
		
		Select @RecCnt = MAX(RowID) FROM #tmpOrderNbrs;
		SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		
		WHILE @RecCnt >= @CurRec 
		Begin
			BEGIN TRANSACTION
			SET NOCOUNT OFF;
				--Prior to insert, set current active record to inactive.
				SELECT @LinkedID = LinkedID FROM dbo.PatientOrder WHERE OrderNumber = @Order
				IF @LinkedID IS NOT NULL
				BEGIN
					UPDATE dbo.PatientOrder SET LinkedID = NULL WHERE OrderNumber = @Order
				END	
			COMMIT TRANSACTION
			SET @CurRec = @CurRec + 1;
			SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		End
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	DROP TABLE #tmpOrderNbrs;		
End
GO
/****** Object:  StoredProcedure [dbo].[InsertProfileUserAssignedSites]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 Nov 2016
-- Description: This sp inserts records into table
--		dbo.Profile_UserAssignedSites
-- =============================================
CREATE PROCEDURE [dbo].[InsertProfileUserAssignedSites]
	@lowerusername VarChar(30),
	@SiteCode VarChar(6),
	@Approved bit

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	Declare @Site VarChar(6) = Null,
		@Approve bit 
		
	-- Check for existing record of this site for this user	
	Select @Site = AssignedSiteCode, @Approve = IsApproved 
	from dbo.Profile_UserAssignedSites
	where LowerUserName = @lowerusername
		and AssignedSiteCode = @SiteCode
		
	-- IF we don't have it, add it	
	If @Site is null
		Begin
			Insert into dbo.Profile_UserAssignedSites
			Values (@lowerusername, @SiteCode, @Approved)
		End
	Else -- we have it, so check if we need to update it
		Begin
			Update dbo.Profile_UserAssignedSites
			Set IsApproved = @Approved
			where LowerUserName = @lowerusername and
				AssignedSiteCode = @SiteCode
		End
	
END
GO
/****** Object:  StoredProcedure [dbo].[InsertScannedRx]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhasen
-- Create date: 15 March 2018
-- Description:	Table to hold scanned Rx's.
-- =============================================
CREATE PROCEDURE [dbo].[InsertScannedRx]
	-- Add the parameters for the stored procedure here
	@IndividualID int,
	@RxID int = 0,
	@DocName VarChar(500) = '',
	@DocType VarChar(50) = '',
	@RxScan VarBinary(Max),
	@Success int OUTPUT,
	@ScannedID INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @ScanDate DateTime,
		@DelDate DateTIme,
		@ErrorLogID int,
		@TempID INT = 0
		
	SELECT @ErrorLogID = 0,
		@Success = 0
		
	Set @ScanDate =  GETDATE()
	Set @DelDate = DATEADD(dd,91,GetDate())	
	
	BEGIN TRY
		BEGIN TRANSACTION
			Insert into dbo.ScannedRx (IndividualID, RxID, DocName, DocType,
				RxScan, ScanDate, DelDate)
			Values (@IndividualID, @RxID, @DocName, @DocType, @RxScan,
				GETDATE(), DATEADD(dd,91,GetDate()))
			
			SET @TempID = @@IDENTITY
			SET @ScannedID = @TempID

			Update dbo.Prescription set RxScanID = @TempID WHERE ID = @RxID
			
		COMMIT TRANSACTION
		SET @Success = 1
	END TRY
	
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
		Set @Success = 0
		SET @ScannedID = 0
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH	

END
GO
/****** Object:  StoredProcedure [dbo].[InsertIndividualType]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 7 November 2014
-- Description:	This sp adds a new record to the
--		IndividualType table
-- Modified: 2/12/16 - baf - to use one call to SP for multiple
--			Individual Types for an Individual
--      2/22/16 - baf - Added DeleteAll parameter
-- =============================================
CREATE PROCEDURE [dbo].[InsertIndividualType]
	-- Add the parameters for the stored procedure here
	@IndividualID	INT,
	@TypeID			VARCHAR(100),
	@IsActive		BIT = 1,
	@DeleteAll		BIT = 0,
	@ModifiedBy		VARCHAR(200),
	@Success		INT OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT = 0, @ID INT, @RecCnt INT = 0, @Patient INT

	CREATE TABLE #IDs (RowID INT, ID VARCHAR(3))

	Select @Patient = ID from dbo.LookupTable where Code = 'INDIVIDUALTYPE' and [TEXT] = 'PATIENT'	
	
	BEGIN 
		IF LEN(@TypeID) > 3
			BEGIN
				
				INSERT INTO #IDs  (RowID, ID)
				SELECT * FROM dbo.SplitStrings(@TypeID,',')	
				
				SELECT @RecCnt = MAX(RowID) FROM #IDs;
			END
		ELSE
			BEGIN
				SET @RecCnt = 1
				SET @ID = CAST(@TypeID AS INT)
			END
	END
	
	BEGIN TRY
			BEGIN TRANSACTION
				-- Deleting existing individual types when called for.
				IF @DeleteAll = 1 
				Begin
					DELETE FROM dbo.IndividualType WHERE IndividualID = @IndividualID AND TypeID <> @Patient
				END
			
				IF @TypeID = ''
				BEGIN
					SET @RecCnt = 0
				END
				
				DECLARE @cnt INT = 1, @PType VARCHAR(20), @Site VARCHAR(6), @SCnt INT = 0
				WHILE @RecCnt >= @cnt
					BEGIN
				-- Add new individual types
					IF @RecCnt > 1
						Begin
							SELECT @ID = CAST(ID AS INT) FROM #IDs WHERE RowID = @cnt
						End

					INSERT INTO dbo.IndividualType (IndividualID, TypeID, IsActive, ModifiedBy)
					VALUES (@IndividualID, @ID, 1, @ModifiedBy)
					
					SELECT @PType = Text FROM dbo.LookupTable WHERE Code = 'INDIVIDUALTYPE' AND ID = @ID
					
					IF @PType <> 'PATIENT'
					BEGIN
						SELECT @Site = SiteCodeID FROM dbo.Individual WHERE ID = @IndividualID
						SELECT @SCnt = COUNT(*) FROM dbo.IndividualIDSiteCodeUnion WHERE IndividualID = @IndividualID AND SiteCode = @Site
						
						IF @SCnt = 0
							BEGIN
								INSERT INTO dbo.IndividualIDSiteCodeUnion ( IndividualID, SiteCode )
								VALUES  ( @IndividualID, @Site)
							END
					END

					SET @Cnt = @Cnt + 1
					SET @SCnt = 0
				END
				SET @Success = 1
			COMMIT TRANSACTION
			
			EXEC dbo.GetIndividualTypesByIndividualID @IndividualID
			DROP TABLE #IDs		
	END TRY
	BEGIN CATCH
			IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
			SET @Success = 0
			RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[GetSRTSwebTotalOrders]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2/20/2015
-- Description:	Retrieves total SRTSweb Order Counts
---		By Clinic
-- =============================================
CREATE PROCEDURE [dbo].[GetSRTSwebTotalOrders]
	-- Add the parameters for the stored procedure here
	@FromDate	Date,
	@ToDate		Date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select distinct Case when (Grouping(sc.SiteCode + ' - ' + sc.SiteName ) = 1) then '     TOTAL ORDER COUNT'
		Else SiteCode + ' - ' + SiteName end Clinic, Count(*) RecCnt
	from dbo.PatientOrder po
		Left Join 
		(Select SiteCode, SiteName from dbo.SiteCode) sc on po.ClinicSiteCode = sc.SiteCode
	where po.MOdifiedBy <> 'SSIS' and ClinicSiteCode <> 'SPURS3'
		and po.DateLastModified between @FromDate and DateAdd(d,1,@ToDate)
	Group by  SiteCode + ' - ' + SiteName, ClinicSiteCode With Rollup
END
GO
/****** Object:  StoredProcedure [dbo].[GetUserEMail]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 18 April 2017
-- Description:	Returns SRTSUser email address
-- Modified:  5/16/2017 - baf - to check for active users
-- =============================================
CREATE PROCEDURE [dbo].[GetUserEMail]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@UserRole VARCHAR(25),
	@Success int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ErrorLogID INT = 0, @ID INT

    	BEGIN TRY
	
		BEGIN TRANSACTION

		select au.LoweredUserName, am.Email, ar.RoleName, puas.AssignedSiteCode
		from aspnet_Membership am
		inner join aspnet_UsersInRoles ur on am.UserId = ur.UserId
		inner join aspnet_Roles ar on ur.RoleId = ar.RoleId
		inner join aspnet_Users au on am.UserId = au.UserId
		inner join Profile_UserAssignedSites puas on au.LoweredUserName =
		puas.LowerUserName
		where am.IsApproved = 1 and (puas.AssignedSiteCode = @siteCode and puas.IsApproved = 1)
		and ar.RoleName = @userRole
		
		SET @Success = 1
		
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
		BEGIN 
			ROLLBACK TRANSACTION;
		END
		
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		SET @Success = 0
		RETURN
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[GetShipToPatientOrders]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2 May 2016
-- Description:	This stored procedure returns the status of
--		ShipToPatient parameter for a list of orders provided.
-- Modified:
-- =============================================
Create PROCEDURE [dbo].[GetShipToPatientOrders]
	-- Add the parameters for the stored procedure here
	@OrderNumber		VARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID	INT = 0,
		@ShipToPatient BIT,
		@RecCnt INT,
		@Order VARCHAR(16),
		@CurRec INT = 1;

	BEGIN TRY
	
		-- Need to loop through the order numbers if there is more than one
		-- Added 3/31/15 -- Check-In/Check-Out Changes

		CREATE TABLE #tmpOrderNbrs
			(RowID int, OrderNbr VarChar(16))
			
		INSERT INTO #tmpOrderNbrs
		SELECT * FROM dbo.SplitStrings(@OrderNumber,',')	

		ALTER TABLE #tmpOrderNbrs
		ADD STP BIT;

		SELECT * FROM #tmpOrderNbrs;
		
		Select @RecCnt = MAX(RowID) FROM #tmpOrderNbrs;
		SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		
		
		while @RecCnt >= @CurRec 
		Begin
			BEGIN TRANSACTION
			SET NOCOUNT OFF;
				--Prior to insert, set current active record to inactive.
				
			SELECT @ShipToPatient = ShiptoPatient FROM dbo.PatientOrder 
			WHERE OrderNumber = @Order
				
			UPDATE #tmpOrderNbrs SET STP = @ShipToPatient WHERE OrderNbr = @Order
				
		    SET NOCOUNT ON;
			COMMIT TRANSACTION
			SET @CurRec = @CurRec + 1;
			SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		End
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
	SELECT OrderNbr, STP FROM #tmpOrderNbrs;
	
	DROP TABLE #tmpOrderNbrs;
END
GO
/****** Object:  StoredProcedure [dbo].[GetScannedRxInfoByID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 March 2018
-- Description:	Retrieve the scanned Rx file info.
-- =============================================
CREATE PROCEDURE [dbo].[GetScannedRxInfoByID]
	-- Add the parameters for the stored procedure here
	@RxID INT,
	@ID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ID, IndividualID, RxID, DocName, DocType, ScanDate,
		DelDate
	FROM dbo.ScannedRx WHERE (RxID IS NULL OR RxID = @RxID) OR ID = @ID
		
END
GO
/****** Object:  StoredProcedure [dbo].[GetScannedRxFileByID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 March 2018
-- Description:	Retrieve the scanned Rx file.
-- =============================================
CREATE PROCEDURE [dbo].[GetScannedRxFileByID]
	-- Add the parameters for the stored procedure here
	@RxID INT,
	@ID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT ID, RxScan FROM dbo.ScannedRx WHERE (RxID IS NULL OR RxID = @RxID) OR ID = @ID;
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetScannedRxByID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 March 2018
-- Description:	Retrieve the scanned Rx file.
-- =============================================
CREATE PROCEDURE [dbo].[GetScannedRxByID]
	-- Add the parameters for the stored procedure here
	@RxID int,
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * from dbo.ScannedRx where RxID = @RxID or ID = @ID
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetRxOrderCount]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 14 May 2015
-- Description:	Retrieves the count of orders for a specific
--		Prescription
-- =============================================
Create PROCEDURE [dbo].[GetRxOrderCount]
	-- Add the parameters for the stored procedure here
	@RxID Int,
	@Cnt Int Output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT @Cnt = COUNT(OrderNumber) from dbo.PatientOrder
	where PrescriptionID = @RxID
END
GO
/****** Object:  StoredProcedure [dbo].[GetProfileInfo]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 17 Nov 2016
-- Description:	This stored procedure retrieves all
--		required data from a users profile
-- =============================================
CREATE PROCEDURE [dbo].[GetProfileInfo]
	@LowerUserName VARCHAR(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @AvailSites VARCHAR(MAX) = ''
	
	Select @AvailSites = COALESCE(@AvailSites, '') + AssignedSiteCode + ' ' + CAST(ISApproved as VarChar(1)) + ','
		FROM dbo.Profile_UserAssignedSites 
		WHERE  LowerUserName = @LowerUserName
		
	
	SELECT IndividualID, psu.LowerUserName, FirstName, MiddleName, LastName, IsCMSUser, PrimarySiteCode, LoggedInSiteCode,
		@AvailSites AS AvailableSiteList
	FROM dbo.Profile_PrimarySiteUnion psu 
	INNER JOIN dbo.Individual ind ON psu.IndividualID = ind.ID
	WHERE psu.LowerUserName = @LowerUserName
END
GO
/****** Object:  UserDefinedFunction [dbo].[fnDecenterCalc]    Script Date: 10/03/2019 13:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 31 July 2013
-- Description:	This procedure will calculate the Decenter value for specified lens and OD/OS values.
--		It must be run each time a value is required, once for near and once for distance, etc.
--		If the result is a Negative Decenter value then Base = OUT, if Positive value then Base = IN,
--			for Prism values, if Negative then VBase is UP, if Positive value then Base is DOWN,
--			or HBase is OUT or IN.  Only the HBase(s) is calculated here.
-- Modified:  3/24/15 - JP - User comments incorporated.
--			 10/23/15 - baf - Modified to correct calculation
-- =============================================
CREATE FUNCTION [dbo].[fnDecenterCalc]
(
	@OrderNumber VarChar(16)
	--@LType varchar(15), @ESize decimal (4,2), @BSize decimal (4,2),
	--@ODDistant decimal (4,2), @OSDistant decimal (4,2),
	--@ODNear decimal (4,2), @OSNear decimal(4,2)
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
Select OrderNumber, ODDDec, ODNDEC, OSDDEC, OSNDEC, LT, FES, FBS, ODPDD, OSPDD, ODPDN, OSPDN,
	Case when ODDDec >= 0 then 'IN' Else 'OUT' end ODDBase,
	Case when OSDDec >= 0 then 'IN' else 'OUT' end OSDBase,
	Case when ODNDec >= 0 then 'IN' else 'OUT' end ODNBase,
	Case when OSNDec >= 0 then 'IN' else 'OUT' end OSNBase
From
(
/*  Calculate Distance Decenter for SVD Lens Types
	Calculate Near Decenter for SVN or SVAL Lens Types
	Calculate both Near and Distance for all others
*/
	Select OrderNumber, LT, FES, FBS, ODPDD, ODPDN, OSPDD, OSPDN, 
	Case when ODPDD > 0 and LT <> 'SVN'  then
		(FES + FBS)/2.0 - ODPDD Else (FES + FBS)/2.0 - ODPDN End ODDDec,
	Case when OSPDD > 0 and LT <> 'SVN'  then 
		(FES + FBS)/2.0 - OSPDD Else (FES + FBS)/2.0 - OSPDN End OSDDec,		
	Case when left(LT,1) != 'S' then
		(FES + FBS)/2.0 - ODPDN  Else 0.00 end ODNDec,
	Case when left(LT,1) != 'S' then
		(FES + FBS)/2.0 - OSPDN  Else 0.00 end OSNDec
	From
		(Select OrderNumber, LensType as LT, CAST(FrameEyeSize as int) as FES,
		CAST(FrameBridgeSize as int) as FBS, pr.ODPDDistant as ODPDD, 
		pr.ODPDNear as ODPDN, pr.OSPDDistant as OSPDD, pr.OSPDNear as OSPDN
		from dbo.PatientOrder po Inner Join
			dbo.Prescription pr on po.PrescriptionID = pr.id
		 where po.OrderNumber = @OrderNumber) a
		)b	
	
	
)
GO
/****** Object:  StoredProcedure [dbo].[GetAllPatientOrdersByIDNumber]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David Blott
-- Create date: 26 nov 2013
-- Description:	Used for G-Eyes (returns all orders for a patient)
-- =============================================
CREATE PROCEDURE [dbo].[GetAllPatientOrdersByIDNumber] 
	
@IDNumber VarChar(150)

AS
BEGIN

SELECT po.OrderNumber, po.DateLastModified
FROM dbo.PatientOrder po 
INNER JOIN dbo.IndividualIDNumber iid on iid.IndividualID = po.IndividualID_Patient
WHERE iid.IDNumber = @IDNumber 
ORDER BY po.DateLastModified DESC

END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPatientInfoByIndividualID]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified: 11/13/2015 - jrp - Removed references to SiteCode on GetPatientOrderByIndividualIdNonGEyes calls.
--		12/11/15 - kb - Removed Exams, Prescriptions and Orders from data pull.
-- =============================================
CREATE PROCEDURE [dbo].[GetAllPatientInfoByIndividualID] 
@IndividualID	INT,
@IsActive bit = 1,
@ModifiedBy VarChar(200),
@SiteCode VARCHAR(6)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	


	if(@IsActive = 1)
	BEGIN	
		EXEC dbo.GetIndividualByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualIDNumbersByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualEMailAddressesByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualPhoneNumbersByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualAddressesByIndividualID @IndividualID, 1, @ModifiedBy
		--EXEC dbo.GetPatientOrderByIndividualIdNonGEyes @IndividualID, @ModifiedBy
		--EXEC dbo.GetExamsByIndividualID @IndividualID, @ModifiedBy
		--EXEC dbo.GetPrescriptionsByIndividualID @IndividualID, @ModifiedBy
		EXEC dbo.GetIndividualTypesByIndividualID @IndividualID
	END
	ELSE
	BEGIN
		EXEC dbo.GetIndividualByIndividualID @IndividualID, 0, @ModifiedBy
		EXEC dbo.GetIndividualIDNumbersByIndividualID @IndividualID, 0, @ModifiedBy
		EXEC dbo.GetIndividualEMailAddressesByIndividualID @IndividualID, 0, @ModifiedBy
		EXEC dbo.GetIndividualPhoneNumbersByIndividualID @IndividualID, 0, @ModifiedBy
		EXEC dbo.GetIndividualAddressesByIndividualID @IndividualID, 0, @ModifiedBy
		--EXEC dbo.GetPatientOrderByIndividualIdNonGEyes @IndividualID, @ModifiedBy
		--EXEC dbo.GetExamsByIndividualID @IndividualID, @ModifiedBy
		--EXEC dbo.GetPrescriptionsByIndividualID @IndividualID, @ModifiedBy
		EXEC dbo.GetIndividualTypesByIndividualID @IndividualID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetClinicSummaryBySiteCode_old]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetClinicSummaryBySiteCode_old]
@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Orders ready for CheckIn from Lab to Clinic
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS ReadyForCheckin 
	FROM dbo.OrderStateHistory a
	INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON b.IndividualID_Patient = c.ID
	WHERE a.ClinicSiteCode = @SiteCode
	AND a.DateClinicReceived IS NULL
	AND a.DateLabDispensed IS NULL
	AND a.DateCancelled IS NULL
	AND a.DateRejected IS NULL
	AND a.DateProductionCompleted IS NOT NULL 
	AND a.IsActive = 1
	AND b.IsGEyes = 0
	GROUP BY a.ClinicSiteCode
	
	-- Orders rejected by Lab
	SELECT ClinicSiteCode, COUNT(ClinicSiteCode) AS Rejected FROM dbo.OrderStateHistory
	WHERE DateRejected IS NOT NULL
	AND DateResubmitted IS NULL
	AND DateCancelled IS NULL
	AND ClinicSiteCode = @SiteCode
	GROUP BY ClinicSiteCode
	
	-- Orders where production complete is 10 or more days greater than date created
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS OverDue 
	FROM dbo.OrderStateHistory a
	INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber
	WHERE a.DateReceivedByLab IS NOT NULL
	AND a.DateProductionCompleted IS NULL
	AND a.DateLabDispensed IS NULL
	AND a.DateClinicReceived IS NULL
	AND a.DateClinicDispensed IS NULL
	AND a.DateRejected IS NULL
	AND a.DateCancelled IS NULL
	AND a.dateRedirected IS NULL
	AND a.DateReceivedByLab < DATEADD(DAY, -10, GETDATE())
	AND a.ClinicSiteCode = @SiteCode
	AND b.IsGEyes = 0
	GROUP BY a.ClinicSiteCode

	-- Average time to dispense - dates between date created and date dispensed average
	SELECT AVG(DATEDIFF(DAY, DateCreated, DateClinicDispensed)) AS AvgDispenseDays FROM dbo.OrderStateHistory
	WHERE DateCreated IS NOT NULL
	AND DateClinicDispensed IS NOT NULL
	AND ClinicSiteCode = @SiteCode
	AND IsActive = 0

	-- gets the number of orders ready for dispense
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS ReadyForDispense 
	FROM dbo.OrderStateHistory a
	INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber
	WHERE a.ClinicSiteCode = @SiteCode
	AND a.DateCreated IS NOT NULL
	AND a.DateProductionCompleted IS NOT NULL
	AND a.DateLabDispensed IS NOT NULL
	AND a.DateClinicReceived IS NOT NULL
	AND a.DateClinicDispensed IS NULL
	AND a.DateCancelled IS NULL
	AND a.IsActive = 1
	AND b.IsGEyes = 0
	GROUP BY a.ClinicSiteCode

END
GO
/****** Object:  StoredProcedure [dbo].[GetAddresses]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 27 July 2017
-- Description:	GetAddresses
-- Modified:  23 August 2017 - baf - split patient's name
--		27 NOV 2017 - baf - Added DateVerified and ExpireDays
-- =============================================
CREATE PROCEDURE [dbo].[GetAddresses]
	-- Add the parameters for the stored procedure here
	@OrderNumber VARCHAR(16)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT OrderNumber, IndividualID_Patient, a.FirstName, a.Middlename, a.LastName, a.Address1, a.Address2,
	a.Address3, a.City, a.State, a.CountryCode, a.CountryName, a.ZipCode, a.DateVerified, a.ExpireDays, 
	po.ShipAddress1, po.ShipAddress2, po.ShipAddress3,po.ShipCity, po.ShipState, po.ShipCountry AS ShipCountryCode, 
	lt.[Text] AS ShipCountryName, po.ShipZipCode
FROM dbo.PatientOrder po
INNER JOIN dbo.LookupTable lt ON po.ShipCountry = lt.Value
LEFT JOIN (
	SELECT ind.ID, FirstName, MiddleName, LastName,ia.Address1, ia.Address2, ia.Address3, ia.City, ia.State, ia.Country AS CountryCode,
			lt2.[Text]AS CountryName, ia.ZipCode,ia.DateVerified, ia.ExpireDays
	FROM dbo.Individual ind
	LEFT JOIN dbo.IndividualAddress ia ON ind.ID = ia.IndividualID
	INNER JOIN dbo.LookupTable lt2 ON ia.Country = lt2.value
	WHERE ind.IsActive = 1 AND ia.IsActive = 1
		AND lt2.Code = 'COUNTRYLIST'
) a ON a.ID = po.IndividualID_Patient
WHERE ORderNumber = @OrderNumber
	AND lt.Code = 'COUNTRYLIST'
END
GO
/****** Object:  StoredProcedure [dbo].[GetActiveUnusedPrescriptionByID]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetActiveUnusedPrescriptionByID]
	@ID			INT, 
	@ModifiedBy	Varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	----------- Audit SingleRead 10/20/14 - BAF
	Declare	@RecID		VarChar(20) = Cast(@ID as VarChar(20))
	Declare	@PatientID 	int
	Declare @ChangeDate DateTime = GetDate()
	
	Select @PatientID = IndividualID_Patient FROM dbo.Prescription
	WHERE ID = @ID and IsActive = 1 
	
	Exec InsertAuditSingleRead @ChangeDate, @PatientID, 11, @ModifiedBy, @RecID
	-------------------

	-- Check to see if this RX is used anywhere
	
	DECLARE @RecCnt INT = 0,
		@IsDeletable Bit;
	
	Select @RecCnt =  Count(*) FROM dbo.PatientOrder WHERE PrescriptionID = @ID AND IsActive = 1
	
	IF @RecCnt > 0 
		Begin
			SET @IsDeletable = 0
		END
	ELSE
		BEGIN
			SET @IsDeletable = 1
		END

	SELECT ID,
	ExamID,
	IndividualID_Patient,
	IndividualID_Doctor,
	ODSphere,
	OSSphere,
	ODCylinder,
	OSCylinder,
	ODAxis,
	OSAxis,
	ODHPrism,
	OSHPrism,
	ODVPrism,
	OSVPrism,
	ODHBase,
	OSHBase,
	ODVBase,
	OSVBase,
	ODAdd,
	OSAdd,
	EnteredODSphere,
	EnteredOSSphere,
	EnteredODCylinder,
	EnteredOSCylinder,
	EnteredODAxis,
	EnteredOSAxis,
	PrescriptionDate,
	IsActive,
	IsUsed,
	ModifiedBy,
	DateLastModified,
	PDNear,
	PDDistant,
	ODPDDistant,
	ODPDNear,
	OSPDDistant,
	OSPDNear,
	IsMonoCalculation,
	IsDeletable = @IsDeletable
	FROM dbo.Prescription
	WHERE ID = @ID
	-- Added following checks 5/27/14 BAF
	and IsActive = 1 --and IsUsed = 0
END
GO
/****** Object:  StoredProcedure [dbo].[GetBillingCodeOrders]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 2/17/2015
-- Description:	Report of orders by VStar Billing Code
-- Modified:  1/4/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetBillingCodeOrders]
	-- Add the parameters for the stored procedure here
	@FromDate DateTime,
	@ToDate DATETIME,
	@BillCode Varchar(4) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select OrderNumber, ClinicSiteCode, po.Demographic, LastName, FirstName, MiddleName, Right(IDNumber,4) as LastFour, 
		IDNumberType as IDType, Value as BillCode, Description as SNMStatus, FrameCode, Tint, Coatings,
		po.DateLastModified, po.ModifiedBy
	From dbo.PatientOrder po
	Left Join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	Left Join dbo.IndividualIDNumber id on id.IndividualID = po.IndividualID_Patient
	Left Join dbo.LookupTable lt on lt.Text = substring(po.demographic,4,3)
	Where po.DateLastModified between @FromDate AND @ToDate
		and id.IsActive = 1 and Value is not NULL
		AND (@BillCode IS NULL OR Value = @BillCode)
	Order by  Description, ClinicSiteCode, FrameCode, DateLastModified
END
GO
/****** Object:  StoredProcedure [dbo].[GetGEyesCounts]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/28/2014
-- Description:	This stored procedure retrives the counts of GEyes orders
--		for Year and Month or full database.
-- Modified:  7/22/15 - baf - Changed Clinic Site Code to '999999' from 'GEYES1'
--	 8/31/15 - baf - Corrected LocationCode pull
-- =============================================
CREATE PROCEDURE [dbo].[GetGEyesCounts] 
	-- Add the parameters for the stored procedure here
	@Year int = Null,
	@Month int = Null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select Theater, YearCreated, MonthCreated, SUM(Cnt) as OrderCnt
From
(
	Select LocationCode as Theater, YEAR(DateLastModified) as YearCreated, MONTH(DateLastModified) as MonthCreated, COUNT(*) as Cnt
	from dbo.PatientOrder
	where ClinicSiteCode = '009900'
	Group by ClinicSiteCode, LocationCode, DateLastModified
)a
where (@Year is Null or YearCreated = @Year)
	and (@Month is Null or MonthCreated = @Month)
Group by Theater, YearCreated, MonthCreated
END
GO
/****** Object:  StoredProcedure [dbo].[GetLabelsOnDemand]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 28 August 2017
-- Description:	Retrieve OrderNumbers for LabelsOnDemand Batch
-- =============================================
CREATE PROCEDURE [dbo].[GetLabelsOnDemand]
	-- Add the parameters for the stored procedure here
	@BatchDate DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

		SELECT DISTINCT lod.OrderNumber, lod.AddressType, ind.FirstName, ind.MiddleName, ind.LastName,
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipAddress1 ELSE ia.Address1 END Address1,
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipAddress2 ELSE ia.Address2 END Address2,
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipAddress3 ELSE ia.Address3 END Address3,
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipCity ELSE ia.City END City,
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipState ELSE ia.[State] END [State],
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipCountry ELSE ia.Country END Country,
			CASE WHEN lod.AddressType = 'ORDER' then
				po.ShipZipCode ELSE ia.ZipCode END ZipCode
		FROM dbo.LabelsOnDemand lod
			INNER JOIN dbo.PatientOrder po ON lod.OrderNumber = po.OrderNumber
			INNER JOIN dbo.Individual ind ON ind.ID = po.IndividualID_Patient
			LEFT JOIN dbo.IndividualAddress ia ON ind.ID = ia.IndividualID
		WHERE BatchDate = @BatchDate and po.IsActive = 1 and ia.IsActive = 1
		
END
GO
/****** Object:  StoredProcedure [dbo].[GetLinkedOrders]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 5/26/2015
-- Description:	This stored procedure retrieves all orders with the same
--		LinkedID
-- =============================================
CREATE PROCEDURE [dbo].[GetLinkedOrders]
	@LinkedID VARCHAR(16)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT OrderNumber FROM dbo.PatientOrder WHERE LinkedID = @LinkedID
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrderEmailMsg]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 08 March 2019
-- Modified 20 March 2019 jrp
-- Description:	Used to retrieve Order EmailMsg
-- =============================================
Create PROCEDURE [dbo].[GetOrderEmailMsg]
	-- Add the parameters for the stored procedure here
	@OrderNumber VARCHAR(16),
	@EmailMsg VARCHAR (2100) OUTPUT,
	@Success INT OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

BEGIN TRY
	DECLARE @ErrorLogID INT = 0,
		    @EmailType VarChar(50) = '',
			@MsgSeg VarChar(50) = '',
			@FrameCategory VarChar(10) = '',
			@Insert Bit,
			@ShipToPatient BIT,
			@ClinicShipToPatient BIT,
			@EmailPatient BIT,
			@Tint VArChar(5),
			@FrameCode VarChar(20)
			
		
		Set @EmailMsg = ''
	
		Select @ShipToPatient = ShipToPatient, @ClinicShipToPatient = ClinicShipToPatient, @FrameCode = FrameCode,
			@EmailPatient = EmailPatient, @Tint = Tint
		From dbo.PatientOrder where OrderNumber = @OrderNumber
		
		If (@ShipToPatient = 1 or @ClinicShipToPatient = 1) and @EmailPatient = 1 
			Begin
				Set @EmailType = 'Order-Mail'
			End
		Else if (@ShipToPatient = 0 and @ClinicShipToPatient = 0) and @EmailPatient = 1
			Begin
				Set @EmailType = 'Order-Clinic'
			End
		
		If @EmailType like 'Order%'
		Begin
			Select @FrameCategory = Category, @Insert = IsInsert from dbo.Frame where FrameCode = @FrameCode
			If @Tint <> 'CL' 
				Begin
					Set @MsgSeg = 'Tinted '
				End
			
			If @FrameCategory = 'Standard' 
				Begin
					Set @MsgSeg = @MsgSeg + 'Standard Issue glasses '
				End
			ELSE If @FrameCategory = 'FOC' 
				Begin
					Set @MsgSeg = @MsgSeg + 'Frame of Choice glasses '
				END
			Else IF @FrameCategory = 'CPE'
				Begin
					Set @MsgSeg = @MsgSeg + 'Protective Eyewear optical insert '
				END
			Else If @FrameCategory = 'PMI'
				Begin
					Set @MsgSeg = @MsgSeg + 'Protective Mask optical insert '
				END
			Else If @FrameCategory = 'Aviator' and @Insert = 1
				Begin
					Set @MsgSeg = @MsgSeg + 'Aviator optical insert '
				End
			Else If @FrameCategory = 'Aviator' and @Insert = 0
				Begin
					Set @MsgSeg = @MsgSeg + 'Aviator glasses '
				END
			Else -- @FrameCategory = 'Non-Stan' or something unknown
				Begin
					Set @MsgSeg = @MsgSeg + 'Non-Standard glasses '
				End
			Select @EmailMsg = convert(varchar(2100),concat(MsgPart1, ' ', @MsgSeg, MsgPart2,' ', MsgPart3, ' ', MsgPart4))
				from dbo.EmailMsg where EmailType = @EmailType
		End
		Set @Success = 1
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN
				-- Select Failed
				SET @Success = 0
			END		
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT		
	END Catch
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrderByIndividualID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		John Shear
 Create date: 31 Oct 2011
 Description:	Returns all order for a patient
 Modified:  12/15/15 - BAF Removed FOCDate
	1/12/17 - baf - Added ClinicShipToPatient
	2/23/17 - baf - Added DispenseComments
	1/04/18 - baf - Added Coatings
	3/19/18 - baf - Added EmailPatient bit
 =============================================*/
CREATE PROCEDURE [dbo].[GetPatientOrderByIndividualID] 
	-- Add the parameters for the stored procedure here
@IndividualID	INT,
@ModifiedBy		VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	----- Audit MultiRead -- 10/17/14 - BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(20) = Cast(@IndividualID as VarChar(20))
	Declare @Notes			VarChar(Max)
	
	Select @Notes = Coalesce (@Notes, 'Order Numbers: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.IndividualPhoneNumber b ON a.PatientPhoneID = b.ID
	INNER JOIN dbo.Frame f ON a.FrameCode = f.FrameCode -- Added for G-Eyes DB 02 Dec 2013
	INNER JOIN dbo.FrameItem fitem1 ON fitem1.Value = a.Tint -- Added for G-Eyes DB 02 Dec 2013
	INNER JOIN dbo.FrameItem fitem2 ON fitem2.Value = a.LensType -- Added for G-Eyes DB 02 Dec 2013
	WHERE IndividualID_Patient = @IndividualID
	AND fitem1.TypeEntry = 'TINT'
	
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	------------------

    -- Insert statements for procedure here
	SELECT 
	a.IndividualID_Patient, 
	a.ClinicSiteCode, 
	a.DateLastModified, 
	a.Demographic, 
	a.FrameBridgeSize, 
	a.FrameCode, 
	a.FrameColor, 
	a.FrameEyeSize, 
	a.FrameTempleType,
	a.IndividualID_Tech, 
	a.IsGEyes, 
	a.IsMultivision, 
	a.LabSiteCode, 
	a.LocationCode, 
	a.ModifiedBy, 
	a.NumberOfCases, 
	a.ODSegHeight, 
	a.Tint, 
	REPLACE(a.Coatings,'/',',') AS Coatings, -- Added Coatings 1/4/18
	a.OrderNumber, 
	a.LensMaterial, 
	a.LensType, 
	a.OSSegHeight, 
	a.Pairs, 
	a.PrescriptionID, 
	a.ShipAddress1, 
	a.ShipAddress2,
	a.ShipAddress3, 
	a.ShipCity, 
	a.ShipState, 
	a.ShipZipCode, 
	a.ShipAddressType, 
	a.ShipCountry, 
	a.ShipToPatient, 
	a.UserComment1, 
	a.UserComment2,
	pr.PDDistant,
	pr.PDNear, 
	pr.ODPDDistant,
	pr.ODPDNear, 
	pr.OSPDDistant,
	pr.OSPDNear,
	pr.IsMonoCalculation,
	a.PatientEmail, 
	--'(' + b.AreaCode + ') ' + b.PhoneNumber AS PatientPhoneNumber,
	b.AreaCode + b.PhoneNumber AS PatientPhoneNumber,
	a.PatientPhoneID,  
	a.OnholdForConfirmation, 
	a.VerifiedBy,
	a.MedProsDispense,
	a.PimrsDispense, 
	a.IsActive, 
	f.FrameDescription, -- Added for G-Eyes DB 02 Dec 2013
	fitem1.[Text] AS 'LensTint', -- Added for G-Eyes DB 02 Dec 2013
	fitem2.[Text] AS 'LensTypeLong', -- Added for G-Eyes DB 02 Dec 2013
	CASE WHEN a.Coatings = 'AR' THEN 'Anti-Refl'
		WHEN a.Coatings = 'UV' THEN 'UV-400'
		WHEN a.coatings = 'AR/UV' THEN 'Anti-Refl,UV-400'
		WHEN a.Coatings = 'UV/AR' THEN 'UV-400,Anti-Refl'
	END LensCoating,
--	FOCDate,
	LinkedID,
	a.ClinicShipToPatient,
	--a.DispenseComments,
	Case when (a.GroupName is not null) or a.Groupname <> '' then	
		a.DispenseComments + ' (' + a.GroupName + ')'
	else a.DispenseComments end DispenseComments,
	a.EmailPatient		-- Added 3/19/18
	FROM dbo.PatientOrder a
	INNER JOIN dbo.Prescription pr ON a.PrescriptionID = pr.ID
	INNER JOIN dbo.IndividualPhoneNumber b ON a.PatientPhoneID = b.ID
	INNER JOIN dbo.Frame f ON a.FrameCode = f.FrameCode -- Added for G-Eyes DB 02 Dec 2013
	INNER JOIN dbo.FrameItem fitem1 ON fitem1.Value = a.Tint -- Added for G-Eyes DB 02 Dec 2013
	INNER JOIN dbo.FrameItem fitem2 ON fitem2.Value = a.LensType -- Added for G-Eyes DB 02 Dec 2013
	Left JOIN dbo.FrameItem fitem3 ON fitem3.value = a.Coatings --added 1/4/18 
	WHERE a.IndividualID_Patient = @IndividualID
	AND fitem1.TypeEntry = 'TINT' AND fitem1.IsActive = 1
	AND (a.Coatings IS NULL OR fitem3.TypeEntry = 'Coating') -- Added 1/4/18
	and a.IsActive = 1
	ORDER BY DateLastModified ASC
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersNoBarCodes]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10/4/2014
-- Description:	This stored procedure selects the 
--		OrderNumbers from the PatientOrder table
--		that do not have a barcode in the record.
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersNoBarCodes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT OrderNumber 
	from dbo.PatientOrder
	Where ONBarCode is null or ONBarCode = ''
	and IsActive = 1
	
END
GO
/****** Object:  StoredProcedure [dbo].[aspnet_Profile_DeleteProfiles]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Profile_DeleteProfiles]
    @ApplicationName        nvarchar(256),
    @UserNames              nvarchar(4000)
AS
BEGIN
    DECLARE @UserName     nvarchar(256)
    DECLARE @CurrentPos   int
    DECLARE @NextPos      int
    DECLARE @NumDeleted   int
    DECLARE @DeletedUser  int
    DECLARE @TranStarted  bit
    DECLARE @ErrorCode    int

    SET @ErrorCode = 0
    SET @CurrentPos = 1
    SET @NumDeleted = 0
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    WHILE (@CurrentPos <= LEN(@UserNames))
    BEGIN
        SELECT @NextPos = CHARINDEX(N',', @UserNames,  @CurrentPos)
        IF (@NextPos = 0 OR @NextPos IS NULL)
            SELECT @NextPos = LEN(@UserNames) + 1

        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPos, @NextPos - @CurrentPos)
        SELECT @CurrentPos = @NextPos+1

        IF (LEN(@UserName) > 0)
        BEGIN
            SELECT @DeletedUser = 0
            EXEC dbo.aspnet_Users_DeleteUser @ApplicationName, @UserName, 4, @DeletedUser OUTPUT
            IF( @@ERROR <> 0 )
            BEGIN
                SET @ErrorCode = -1
                GOTO Cleanup
            END
            IF (@DeletedUser <> 0)
                SELECT @NumDeleted = @NumDeleted + 1
        END
    END
    SELECT @NumDeleted
    IF (@TranStarted = 1)
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END
    SET @TranStarted = 0

    RETURN 0

Cleanup:
    IF (@TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END
    RETURN @ErrorCode
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteScannedRx]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 March 2018
-- Description:	Used to delete a Scanned Rx from the table
-- =============================================
CREATE PROCEDURE [dbo].[DeleteScannedRx]
	-- Add the parameters for the stored procedure here
	@ID int = 0,
	@DelScanDate DateTime
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @DelDate Date,
		@Rx int = 0
	
	Set @DelDate = Cast(@DelScanDate as Date)
	
	If @ID <> 0 
	-- Delete the record with this ID Number
		BEGIN
			Select @Rx = RxID from dbo.ScannedRx where ID = @ID
			Delete from dbo.ScannedRx where ID = @ID
			Update dbo.Prescription set RxScanID = 0 where ID = @Rx
		END
	ELSE
		-- Delete all records for this diate
		BEGIN
			-- Load Temp Table
			Create Table #tmpDelDates (ID int, RxID int)
			Insert into #tmpDelDates
			Select ID, RxID from dbo.ScannedRx where DelDate = @DelDate
	
			Delete from dbo.ScannedRx where ID in
				(Select ID from #tmpDelDates)
				
			Update dbo.Prescription set RxScanID = 0 where ID in 
				(Select RxID from #tmpDelDates)
				
			Drop Table #tmpDelDates		
		END
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteProfile_UserAssignedSites]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 Nov 2016
-- Description:	Deletes a record from dbo.Profile_UserAssignedSites
-- =============================================
CREATE PROCEDURE [dbo].[DeleteProfile_UserAssignedSites]
	-- Add the parameters for the stored procedure here
	@LowerUser VarChar(30),
	@AssignedSiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Delete from dbo.Profile_UserAssignedSites
	where LowerUserName = @LowerUser and
		AssignedSiteCode = @AssignedSiteCode

END
GO
/****** Object:  StoredProcedure [dbo].[DeleteClass]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Travis Hawk
-- Create date: <Create Date,,>
-- Description:	This stored procedure clears out the
--		database to get it ready for the next training class.
--  Modified:  8/11/14 - kdb to ensure proper deletion path
-- =============================================
CREATE PROCEDURE [dbo].[DeleteClass] 
@ClassCode	VARCHAR(5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	/*
		To delete orders this is the order:
		1. Delete PatientOrderStatus
		2. Delete PatientOrder
		3. Delete Prescription
		4. Delete Exam

		
		To delete individuals this is the order:
		1. Delete Individual
		2. Delete IndividualAddress
		3. Delete IndividualEmailAddress
		4. Delete IndividualIDNumber
		5. Delete IndividualPhoneNumber
	*/
	
	DELETE dbo.PatientOrderStatus
	WHERE LabSiteCode LIKE @ClassCode + '%'
	
	DELETE dbo.PatientOrder
	WHERE ClinicSiteCode LIKE @ClassCode + '%'

	-- Delete Exam and Prescription
	DECLARE MyCursor1 Cursor
	FOR
		SELECT ID  FROM dbo.Individual
		WHERE SiteCodeID LIKE @ClassCode + 'C' OR SiteCodeID LIKE @ClassCode + 'L'

		OPEN MyCursor1

		DECLARE @ID varchar(15)

		fetch next from MyCursor1 into @ID
		WHILE(@@FETCH_STATUS <> -1)
		BEGIN
	
			DELETE dbo.Prescription
			WHERE IndividualID_Patient = @ID
 
			DELETE dbo.Exam
			WHERE IndividualID_Patient = @ID

			-- Since the Individual.FirstName is the same as the aspnet_users.UserName and 
			-- aspnet_users.UserName is the same as [Authorization].UserName
			-- we will get the FirstName via select with the @ID and use that to 
			-- delete the users record in the [Authorization] table
			DELETE FROM [Authorization]
			WHERE UserName = (SELECT FirstName FROM Individual t WHERE t.ID = @ID)

			DELETE FROM [Authorization]
			WHERE SSO_UserName = (SELECT FirstName FROM Individual t WHERE t.ID = @ID)
			
		fetch next from MyCursor1 into @ID
		END
	close MyCursor1
	DEALLOCATE MyCursor1
	
	-- Delete Individual, IndividualAddress, IndividualEmailAddress, IndividualIDNumber, 
	--	and IndividualPhoneNumber
	DECLARE MyCursor2 Cursor
	FOR
		SELECT ID  FROM dbo.Individual
		WHERE SiteCodeID LIKE @ClassCode + 'C' OR SiteCodeID LIKE @ClassCode + 'L'

		OPEN MyCursor2

		DECLARE @ID2 varchar(15)

		fetch next from MyCursor2 into @ID2
		WHILE(@@FETCH_STATUS <> -1)
		BEGIN
			DELETE dbo.IndividualType
			WHERE IndividualID = @ID2
		
			DELETE dbo.IndividualAddress
			WHERE IndividualID = @ID2
 
			DELETE dbo.IndividualEMailAddress
			WHERE IndividualID = @ID2
			
			DELETE dbo.IndividualIDNumber
			WHERE IndividualID = @ID2
			
			DELETE dbo.IndividualPhoneNumber
			WHERE IndividualID = @ID2
			
			DELETE dbo.IndividualIDSiteCodeUnion
			WHERE IndividualID = @ID2
			
			DELETE dbo.Individual
			WHERE ID = @ID2
			
			fetch next from MyCursor2 into @ID2
		END
	close MyCursor2
	DEALLOCATE MyCursor2
	
	-- Delete SiteAddress
	DELETE dbo.SiteAddress
	WHERE SiteCode = @ClassCode + 'L' OR SiteCode = @ClassCode + 'C'
	
	-- Delete SiteCode	
	DELETE dbo.SiteCode
	WHERE SiteCode = @ClassCode + 'L' OR SiteCode = @ClassCode + 'C'
	
	-- Delete User Information
	Declare @Class VarChar(6) = @ClassCode + '%'
	
	DECLARE MyCursor3 Cursor
	FOR
		SELECT UserID  FROM dbo.aspnet_Users
		WHERE UserName like @Class

		OPEN MyCursor3

		DECLARE @ID3 UniqueIdentifier

		fetch next from MyCursor3 into @ID3
		WHILE(@@FETCH_STATUS <> -1)
		BEGIN
		
		Delete from dbo.aspnet_UsersInRoles where UserId = @ID3
		Delete from dbo.aspnet_Profile where UserId  = @ID3
		Delete from dbo.aspnet_Membership where UserId = @ID3
		Delete from dbo.aspnet_Users where UserId = @ID3
		
			fetch next from MyCursor3 into @ID3
		END
	close MyCursor3
	DEALLOCATE MyCursor3
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersInCompleteByClinicCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersInCompleteByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--------------- Audit MultiRead - 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 15 
	AND b.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
	SELECT @Notes = COALESCE(@Notes, 'Incomplete Order Numbers: ') + a.OrderNumber  + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 15 
	AND b.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	------------------------------------------------
	SELECT * FROM dbo.PatientOrder a
	INNER JOIN dbo.Prescription pr ON pr.ID = a.PrescriptionID
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 15 
	AND b.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	and a.IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForPIMRSSinceLast]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  5/13/16 - baf - Modified to pull correct dates, and bit
--		changed for when record sent to PIMRS
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForPIMRSSinceLast] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--First create temporary table to hold order numbers and date.	
	CREATE TABLE #tmpOrders
	(OrderNbr VARCHAR(16), OrderDate DATETIME, RecDate DATETIME, PimrsFlag INT)
	INSERT INTO #tmpOrders
	SELECT po.OrderNumber, posc.DateLastModified AS d_ord, posr.DateLastModified AS d_recd, po.PimrsDispense 
	FROM dbo.PatientORder po
		INNER JOIN dbo.PatientOrderStatus posc ON po.Ordernumber = posc.OrderNumber
		LEFT JOIN dbo.PatientOrderStatus posr ON po.ORDErNumber = posr.OrderNumber AND ((posr.OrderStatusTypeID IN (8,19)) OR
			(posr.OrderStatusTypeID = 7 AND posr.StatusComment LIKE '%LMS%'))
	WHERE po.IsActive = 1 
		AND substring(po.Demographic,4,1) = 'F'
		AND (po.PimrsDispense IS NULL OR po.PimrsDispense < 3) 
		and po.ModifiedBy <> 'SSIS';


	-- Select data for for ASIMS
	SELECT a.PimrsDispense,
	'' as orderkey  -- Need to find a way to match this to what is in the legacy system
	,a.Ordernumber as ordernum  -- This is also different than what is in the legacy system
	,b.IDNumber as ssn
	,c.DateLastModified as d_ord	,pos.DateLastModified as d_recd	,e.DateLastModified as d_rx
	,e.ODSphere as odsph	,e.ODCylinder as odcyl	,e.ODAxis as odaxis	,e.ODHPrism as odhprism	,e.ODHBase as odhbase
	,e.ODVPrism as odvprism	,e.ODVBase as odvbase	,e.ODAdd as odadd	,a.ODSegHeight as odseght	,e.OSSphere as ossph
	,e.OSCylinder as oscyl	,e.OSAxis as osaxis	,e.OSHPrism as oshprism	,e.OSHBase as oshbase	,e.OSVPrism as osvprism
	,e.OSVBase as osvbase	,e.OSAdd as osadd	,a.OSSegHeight as osseght	,a.Tint as tint	,a.FrameCode as frame
	,a.Pairs as pair	,a.LensType as lenstype	,e.PDDistant as distpd	,e.PDNear as nearpd	
	,f.ODUncorrectedAcuity as oduncorrected	,f.OSUncorrectedAcuity as osuncorrected
	,f.ODOSUncorrectedAcuity as bothuncorrected	,f.ODCorrectedAcuity as odcorrected
	,f.OSCorrectedAcuity as oscorrected	,f.ODOSCorrectedAcuity as bothcorrected
	,f.ExamDate as d_acuitytest	,a.DateLastModified as d_modified 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.IndividualIDNumber b ON a.IndividualID_Patient = b.IndividualID
	INNER JOIN dbo.Individual ind ON ind.ID = a.IndividualID_Patient
	INNER JOIN dbo.PatientOrderStatus c on a.OrderNumber = c.OrderNumber AND c.OrderStatusTypeID = 1
	LEFT JOIN dbo.PatientOrderStatus pos ON pos.OrderNumber = a.OrderNumber AND ((pos.OrderStatusTypeID IN (8,19)) OR
		(pos.OrderStatusTypeID = 7 AND pos.StatusComment LIKE '%LMS%'))
	INNER JOIN dbo.Prescription e on a.PrescriptionID = e.ID
	Left JOIN dbo.Exam f on e.ExamID = f.ID 
	WHERE SUBSTRING(ind.Demographic,4,1) = 'F'
	and a.IsActive = 1
	AND (a.PimrsDispense IS NULL OR a.PimrsDispense < 3)
	AND a.ModifiedBy <> 'SSIS';
	

	-- Update the PIMRS/ASIMS DISPENSE FLAG Accordingly
	UPDATE dbo.PatientOrder SET PimrsDispense = 2
	FROM dbo.PatientOrder po INNER JOIN #tmpOrders tmpO ON po.OrderNumber = tmpO.OrderNbr
	WHERE tmpO.RecDate IS NOT NULL;

	UPDATE dbo.PatientOrder SET PimrsDispense = 1
	FROM dbo.PatientOrder po INNER JOIN #tmpOrders tmpO ON po.OrderNumber = tmpO.OrderNbr
	WHERE tmpO.RecDate IS NULL;
    
	DROP TABLE #tmpOrders;
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForPIMRSByDate]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForPIMRSByDate]
@StartDate	DateTime,
@EndDate	DateTime 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT 
	'' as orderkey  -- Need to find a way to match this to what is in the legacy system
	,a.Ordernumber as ordernum  -- This is also different than what is in the legacy system
	,b.IDNumber as ssn
	,c.DateLastModified as d_ord
	,c.DateLastModified as d_recd
	,e.DateLastModified as d_rx
	,e.ODSphere as odsph
	,e.ODCylinder as odcyl
	,e.ODAxis as odaxis
	,e.ODHPrism as odhprism
	,e.ODHBase as odhbase
	,e.ODVPrism as odvprism
	,e.ODVBase as odvbase
	,e.ODAdd as odadd
	,a.ODSegHeight as odseght
	,e.OSSphere as ossph
	,e.OSCylinder as oscyl
	,e.OSAxis as osaxis
	,e.OSHPrism as oshprism
	,e.OSHBase as oshbase
	,e.OSVPrism as osvprism
	,e.OSVBase as osvbase
	,e.OSAdd as osadd
	,a.OSSegHeight as osseght
	,a.Tint as tint
	,a.FrameCode as frame
	,a.Pairs as pair
	,a.LensType as lenstype
	,e.PDDistant as distpd
	,e.PDNear as nearpd
	,f.ODUncorrectedAcuity as oduncorrected
	,f.OSUncorrectedAcuity as osuncorrected
	,f.ODOSUncorrectedAcuity as bothuncorrected
	,f.ODCorrectedAcuity as odcorrected
	,f.OSCorrectedAcuity as oscorrected
	,f.ODOSCorrectedAcuity as bothcorrected
	,f.ExamDate as d_acuitytest
	,a.DateLastModified as d_modified 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.IndividualIDNumber b ON a.IndividualID_Patient = b.IndividualID
	INNER JOIN dbo.PatientOrderStatus c on a.OrderNumber = c.OrderNumber AND c.OrderStatusTypeID = 1
	INNER JOIN dbo.Prescription e on a.PrescriptionID = e.ID
	LEFT OUTER JOIN dbo.Exam f on e.ExamID = f.ID 
	WHERE a.DateLastModified BETWEEN @StartDate AND @EndDate
	and a.IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForOverDueProductionByClinicCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified: 2/26/15 - BAF - Use fnAddBizDays to take into account
--		weekends and holidays
--	3/5/2015 - BAF - Corrected calculation
--    5/20/15 - baf - modified calculation to utilize new date requirements
--			provided by the BAMC lab.
--	3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForOverDueProductionByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--------------- Audit MultiRead - 10/27/14 - BAF
	DECLARE	 @ReadDate		DATETIME = GETDATE()
	DECLARE	 @PatientList	VARCHAR(MAX)
	DECLARE  @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	      ,@Notes = COALESCE(@Notes, 'Order Numbers To Dispense: ') + a.OrderNumber  + ', '
	FROM dbo.PatientOrderStatus t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber 
	INNER JOIN dbo.Individual b ON a.IndividualID_Patient = b.ID 
	WHERE t.OrderStatusTypeID = 2 
	AND a.ClinicSiteCode = @ClinicSiteCode
	AND t.OrderNumber = a.OrderNumber
	AND a.IsActive = 1
	AND t.IsActive = 1
	AND a.IsGEyes = 0
	AND DATEDIFF(DAY, dbo.fn_AddBizDays(t.DateLastModified,10), GETDATE()) >= 1                                               
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	------------------------------------------------
Select OrderNumber, dbo.fnWrkDays(OrderDueDate, GETDATE()) as DaysPastDue,
	DateReceivedByLab, ClinicSiteCode, LabSiteCode, StatusComment, OrderStatusTypeID,
	ModifiedBy, IsActive, FrameCode, LensType, LensMaterial, IndividualID, LastName, MiddleName,
	FirstName
From
	(
	SELECT 
		s.OrderNumber,
		CASE WHEN a.IsMultivision = 0 and	SUBSTRING(a.Demographic,8,1) = 'R' 
				THEN dbo.fn_AddBizDays(s.DateLastModified,2)
				WHEN a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,5)	
				WHEN a.IsMultivision = 0 and SUBSTRING(a.Demographic,8,1) = 'T' 
					then dbo.fn_AddBizDays(s.DateLastModified,2)
				WHEN a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'T' 
					Then dbo.fn_AddBizDays(s.DateLastModified,5)	
				WHEN a.IsMultivision = 0 and SUBSTRING(a.Demographic,8,1) = 'P' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,1)
				When a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'P'
					then dbo.fn_AddBizDays(s.DateLastModified,4)
				WHEN a.IsMultivision = 0 and SUBSTRING(a.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,1)
				WHEN a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,1)					
				WHEN a.IsMultivision = 0 and SUBSTRING(a.Demographic,8,1) = 'S' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,2)
				When a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'S'
					then dbo.fn_AddBizDays(s.DateLastModified,5)
				WHEN a.IsMultivision = 0 and SUBSTRING(a.Demographic,8,1) = 'F' 
					Then dbo.fn_AddBizDays(s.DateLastModified,2)
				When a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'F'
					then dbo.fn_AddBizDays(s.DateLastModified,5)
				WHEN a.IsMultivision = 0 and SUBSTRING(a.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,1)
				WHEN a.IsMultivision = 1 and SUBSTRING(a.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(s.DateLastModified,4)
			END OrderDueDate,--, 5 as MaxShipDays
		--DATEDIFF(DAY, dbo.fn_AddBizDays(s.DateLastModified,10), GETDATE()) AS DaysPastDue,
	--DATEDIFF(DAY, s.DateLastModified, GETDATE()) AS DaysPastDue,
		s.DateLastModified AS 'DateReceivedByLab', 		a.ClinicSiteCode, 		s.LabSiteCode, 
		s.StatusComment,		s.OrderStatusTypeID,  		s.ModifiedBy,		a.IsActive, 
		a.FrameCode,		a.LensType,		a.LensMaterial, b.ID AS IndividualID,		b.LastName,
		b.MiddleName,		b.FirstName	, a.EmailPatient
	FROM 		dbo.PatientOrder a 
		inner join PatientOrderStatus s on a.OrderNumber = s.OrderNumber
		INNER JOIN dbo.Individual b ON a.IndividualID_Patient = b.ID 
	WHERE (s.OrderStatusTypeID = 2 AND s.IsActive = 1) AND
		a.ClinicSiteCode = @ClinicSiteCode AND 
		a.IsActive = 1 AND 
		a.IsGEyes = 0 AND 
		--DATEDIFF(day, s.DateLastModified, GETDATE()) >= 10
		DATEDIFF(DAY, dbo.fn_AddBizDays(s.DateLastModified,10), GETDATE()) >= 1
		) po
ORDER BY DaysPastDue DESC, OrderNumber
	

END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForMedProsOrders]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPatientOrdersForMedProsOrders] 
-- Modified By: Kevin Bush
-- Modified On: 18 Apr 14
-- Modified: jrp 4/19/2018 to return ssn and edipi
-- Description:
--	This parameter has a default value of wildcard (%).  
--	If you do not have a value to provide from the code when calling this SP 
--	do NOT include a blank parameter in the call.  Just leave the parameter out of the call.
@OrderID VARCHAR(16) = '%'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		-----  Audit Multi Read -- 10/16/14 - BAF
		--- Multiple Orders, Multiple Patients
		Declare @ReadDate	DateTime = GetDate()
		Declare @Patient	varchar(Max) 
		Declare @Notes		VarChar(Max)
		Declare @ModifiedBy	VarChar(200) = 'WebService'
		
		SELECT @Notes = Coalesce(@Notes, 'Order Numbers: ') + l.OrderNumber + ', '
		FROM PatientOrderStatus a
		INNER JOIN dbo.PatientOrder l ON l.OrderNumber = a.OrderNumber
		LEFT OUTER JOIN dbo.FrameItem b ON l.LensType = b.Value
		LEFT OUTER JOIN dbo.FrameItem c ON l.LensMaterial = c.Value
		LEFT OUTER JOIN dbo.FrameItem e ON l.FrameColor = e.Value
		LEFT OUTER JOIN dbo.FrameItem f ON l.Tint = f.Value
		LEFT OUTER JOIN dbo.FrameItem i ON l.FrameTempleType = i.Value
		INNER JOIN dbo.Frame d ON l.FrameCode = d.FrameCode
		INNER JOIN dbo.Individual g ON l.IndividualID_Patient = g.ID
		INNER JOIN dbo.Individual h ON l.IndividualID_Tech = h.ID
		INNER JOIN dbo.Prescription j ON l.PrescriptionID = j.ID
		INNER JOIN dbo.Individual m ON j.IndividualID_Doctor = m.ID
		INNER JOIN dbo.IndividualIDNumber k ON l.IndividualID_Patient = k.IndividualID
		WHERE a.OrderStatusTypeID = 1 
		and a.DateLastModified > DATEADD(YEAR, -3, GETDATE()) 
		AND a.IsActive = 1
		AND (a.OrderNumber LIKE @OrderID) 
			
		SELECT @Patient = Coalesce(@Patient, '') + Cast(l.IndividualID_Patient as VarChar(20)) + ', '
		FROM PatientOrderStatus a
		INNER JOIN dbo.PatientOrder l ON l.OrderNumber = a.OrderNumber
		LEFT OUTER JOIN dbo.FrameItem b ON l.LensType = b.Value
		LEFT OUTER JOIN dbo.FrameItem c ON l.LensMaterial = c.Value
		LEFT OUTER JOIN dbo.FrameItem e ON l.FrameColor = e.Value
		LEFT OUTER JOIN dbo.FrameItem f ON l.Tint = f.Value
		LEFT OUTER JOIN dbo.FrameItem i ON l.FrameTempleType = i.Value
		INNER JOIN dbo.Frame d ON l.FrameCode = d.FrameCode
		INNER JOIN dbo.Individual g ON l.IndividualID_Patient = g.ID
		INNER JOIN dbo.Individual h ON l.IndividualID_Tech = h.ID
		INNER JOIN dbo.Prescription j ON l.PrescriptionID = j.ID
		INNER JOIN dbo.Individual m ON j.IndividualID_Doctor = m.ID
		INNER JOIN dbo.IndividualIDNumber k ON l.IndividualID_Patient = k.IndividualID
		WHERE a.OrderStatusTypeID = 1 
		and a.DateLastModified > DATEADD(YEAR, -3, GETDATE()) 
		AND a.IsActive = 1
		AND (a.OrderNumber LIKE @OrderID) 

		If @Notes is not null
			Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 3, @Patient, @Notes
		
		----------------------------------------------
	
		SELECT a.OrderNumber,
		l.FrameBridgeSize AS bridgesizecode,
		m.LastName + ', ' + m.FirstName AS doctorname, 
		l.FrameEyeSize AS eyesizecode,
		l.FrameCode AS framecode, 
		d.FrameDescription AS framedescription,
		l.FrameColor AS framecolorcode,
		e.[Text] AS framecolordescription,
		l.Tint AS tintcode,
		f.[Text] AS tintdescription,
		l.LensType AS lenstypecode,
		b.[Text] AS lenstypedescription,
		g.LastName AS lastname,
		g.FirstName AS firstname,
		SUBSTRING(g.Demographic, 7, 1) AS sexcode,
		REPLACE(SUBSTRING(g.Demographic, 1, 3), '*', '') AS rankcode,
		l.LensMaterial AS lensmaterialcode,
		c.[Text] AS lensmaterialdescription,
		SUBSTRING(g.Demographic, 5,2) AS statuscode,
		SUBSTRING(h.FirstName, 1,1) + SUBSTRING(h.MiddleName, 1,1) + SUBSTRING(h.LastName,1,1) AS technicianinitials,
		l.FrameTempleType AS templecode,
		i.[Text] AS templedescription,
		l.NumberOfCases AS cases,
		l.UserComment1 AS comment1,
		l.UserComment2 AS comment2,
		'' AS comment3,
		'' AS comment4,
		'' AS comment5,
		'' AS comment6,
		l.Pairs AS pair,
		'' AS watermarktest1,
		'' AS watermarktext2,
		a.DateLastModified AS odereddt,
		j.PDDistant AS pddistancecode,
		j.PDNear AS pdnearcode,
		j.ODAdd AS odaddpowercode,
		j.OSAdd AS osaddpowercode,
		j.ODAxis AS odaxiscode,
		j.OSAxis AS osaxiscode,
		j.ODCylinder AS odcylcode,
		j.OSCylinder AS oscylcode,
		j.ODHBase AS odhbasecode,
		j.OSHBase AS oshbasecode,
		j.ODHPrism AS odhprismcode,		
		j.OSHPrism AS oshprismcode,
		l.ODSegHeight AS odsegmentheightcode,
		l.OSSegHeight AS ossegmentheightcode,
		j.ODSphere AS odspherecode,
		j.OSSphere AS osspherecode,
		j.ODVBase AS odvbasecode,
		j.OSVBase AS osvbasecode,
		j.ODVPrism AS odvprismcode,
		j.OSVPrism AS osvprismcode,
		ssn.IDNumber AS ssn, -- Modified jrp 4/19/2018
		din.IDNumber AS din, -- Modified jrp 4/19/2018
		'' AS oddecentercode,
		'' AS osdecentercode,
		'' AS oddecenterbase,
		'' AS osdecenterbase,
		'' AS odtotaldecenter,
		'' AS ostotaldecenter,
		'' AS odtotaldecenterbase,
		'' AS ostotaldecenterbase
		FROM PatientOrderStatus a
		INNER JOIN dbo.PatientOrder l ON l.OrderNumber = a.OrderNumber
		LEFT OUTER JOIN dbo.FrameItem b ON l.LensType = b.Value
		LEFT OUTER JOIN dbo.FrameItem c ON l.LensMaterial = c.Value
		LEFT OUTER JOIN dbo.FrameItem e ON l.FrameColor = e.Value
		LEFT OUTER JOIN dbo.FrameItem f ON l.Tint = f.Value
		LEFT OUTER JOIN dbo.FrameItem i ON l.FrameTempleType = i.Value
		INNER JOIN dbo.Frame d ON l.FrameCode = d.FrameCode
		INNER JOIN dbo.Individual g ON l.IndividualID_Patient = g.ID
		INNER JOIN dbo.Individual h ON l.IndividualID_Tech = h.ID
		INNER JOIN dbo.Prescription j ON l.PrescriptionID = j.ID
		INNER JOIN dbo.Individual m ON j.IndividualID_Doctor = m.ID
    	left outer join dbo.IndividualIDNumber ssn on ssn.IndividualID=l.IndividualID_Patient and ssn.IDNumberType='SSN' and ssn.IsActive=1 -- Modified jrp 4/19/2018
	    left outer join dbo.IndividualIDNumber din on din.IndividualID=l.IndividualID_Patient and din.IDNumberType='DIN' and din.IsActive=1	-- Modified jrp 4/19/2018	
		WHERE a.OrderStatusTypeID = 1 
		and a.DateLastModified > DATEADD(YEAR, -3, GETDATE()) 
		AND (a.OrderNumber LIKE @OrderID) 
		and l.IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForMedProsFirstPass]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:    Kevin Bush - 4/13/15 - Added check
--		for DIN as well as SSN.
--              jrp - 4/19/2018 - removed SSN and combined search for either SSN or EDI
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForMedProsFirstPass] 
@edipi  VARCHAR(10) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	------ Audit MultiRead Routine -- 10/16/14 - BAF
	---  Multiple Records for one patient
	Declare @ReadDate		DateTime = GetDate()
	Declare @ModifiedBy		VarChar(200) = 'WebService'
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)

	Select @Notes = Coalesce (@Notes, 'MEDPROS FirstPass: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.Frame b ON a.FrameCode = b.FrameCode
	INNER JOIN dbo.FrameItem c ON a.Tint = c.Value
	INNER JOIN dbo.PatientOrderStatus d ON a.OrderNumber = d.OrderNumber
	INNER JOIN dbo.FrameItem e ON a.LensType = e.Value
	INNER JOIN dbo.Individual f ON a.IndividualID_Patient = f.ID
	INNER JOIN dbo.IndividualIDNumber g ON a.IndividualID_Patient = g.IndividualID
	WHERE d.DateLastModified > DATEADD(YEAR, -3, GETDATE()) and d.OrderStatusTypeID = 1
	AND (g.IDNumber = @edipi AND (g.IDNumberType = 'SSN' OR g.IDNumberType = 'DIN'))
	
	Select @PatientList = IndividualID 
	from dbo.IndividualIDNumber 
	where (IDNumber = @edipi AND (IDNumberType = 'SSN' OR IDNumberType = 'DIN'))

	
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 3, @PatientList, @Notes
	----------------------------
	
	SELECT DISTINCT
	a.OrderNumber,
	ssn.IDNumber as ssn, -- modified jrp 4/19/2018
	din.IDNumber as din, -- modified jrp 4/19/2018
	b.FrameDescription AS framedescription,
	a.FrameCode AS framecode,
	a.FrameColor AS framecolorcode,
	c.[Text] AS tintdescription,
	d.DateLastModified AS ordereddt,
	e.[Text] AS lenstypedescription,
	f.LastName AS lastname,
	f.FirstName AS firstname,
	SUBSTRING(a.Demographic,7,1) AS sexcode
	FROM dbo.PatientOrder a
	INNER JOIN dbo.Frame b ON a.FrameCode = b.FrameCode
	INNER JOIN dbo.FrameItem c ON a.Tint = c.Value
	INNER JOIN dbo.PatientOrderStatus d ON a.OrderNumber = d.OrderNumber
	INNER JOIN dbo.FrameItem e ON a.LensType = e.Value
	INNER JOIN dbo.Individual f ON a.IndividualID_Patient = f.ID
	left outer join dbo.IndividualIDNumber ssn on ssn.IndividualID=a.IndividualID_Patient and ssn.IDNumberType='SSN' and ssn.IsActive=1 -- modified jrp 4/19/2018
	left outer join dbo.IndividualIDNumber din on din.IndividualID=a.IndividualID_Patient and din.IDNumberType='DIN' and din.IsActive=1 -- modified jrp 4/19/2018
	WHERE d.DateLastModified > DATEADD(YEAR, -3, GETDATE()) and d.OrderStatusTypeID = 1
	AND (ssn.IDNumber = @edipi OR din.IDNumber=@edipi) -- modified jrp 4/19/2018
	and a.IsActive = 1 
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForMedProsDateRange]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 4/13/2015
-- Modified: by jrp 4/19/2018 to return ssn and edipi
-- Description:	Based upon MEDPros Data Pull by Order Number
--		This stored procedure pulls according to a date
--		range.
-- =============================================

CREATE PROCEDURE [dbo].[GetPatientOrdersForMedProsDateRange] 
@BeginDate	DATETIME,
@EndDate	DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		-----  Audit Multi Read -- 10/16/14 - BAF
		--- Multiple Orders, Multiple Patients
		Declare @ReadDate	DateTime = GetDate()
		Declare @Patient	varchar(Max) 
		Declare @Notes		VarChar(Max)
		Declare @ModifiedBy	VarChar(200) = 'WebService'
		
		SELECT @Notes = Coalesce(@Notes, 'Order Numbers: ') + l.OrderNumber + ', '
		FROM PatientOrderStatus a
		INNER JOIN dbo.PatientOrder l ON l.OrderNumber = a.OrderNumber
		LEFT OUTER JOIN dbo.FrameItem b ON l.LensType = b.Value
		LEFT OUTER JOIN dbo.FrameItem c ON l.LensMaterial = c.Value
		LEFT OUTER JOIN dbo.FrameItem e ON l.FrameColor = e.Value
		LEFT OUTER JOIN dbo.FrameItem f ON l.Tint = f.Value
		LEFT OUTER JOIN dbo.FrameItem i ON l.FrameTempleType = i.Value
		INNER JOIN dbo.Frame d ON l.FrameCode = d.FrameCode
		INNER JOIN dbo.Individual g ON l.IndividualID_Patient = g.ID
		INNER JOIN dbo.Individual h ON l.IndividualID_Tech = h.ID
		INNER JOIN dbo.Prescription j ON l.PrescriptionID = j.ID
		INNER JOIN dbo.Individual m ON j.IndividualID_Doctor = m.ID
		INNER JOIN dbo.IndividualIDNumber k ON l.IndividualID_Patient = k.IndividualID
		WHERE a.OrderStatusTypeID = 1 
		--and a.DateLastModified > DATEADD(YEAR, -3, GETDATE()) 
		AND @EndDate <= DateAdd(mm,12,@BeginDate)
		AND a.IsActive = 1
		AND a.DateLastModified BETWEEN @BeginDate AND @EndDate;
			
		SELECT @Patient = Coalesce(@Patient, '') + Cast(l.IndividualID_Patient as VarChar(20)) + ', '
		FROM PatientOrderStatus a
		INNER JOIN dbo.PatientOrder l ON l.OrderNumber = a.OrderNumber
		LEFT OUTER JOIN dbo.FrameItem b ON l.LensType = b.Value
		LEFT OUTER JOIN dbo.FrameItem c ON l.LensMaterial = c.Value
		LEFT OUTER JOIN dbo.FrameItem e ON l.FrameColor = e.Value
		LEFT OUTER JOIN dbo.FrameItem f ON l.Tint = f.Value
		LEFT OUTER JOIN dbo.FrameItem i ON l.FrameTempleType = i.Value
		INNER JOIN dbo.Frame d ON l.FrameCode = d.FrameCode
		INNER JOIN dbo.Individual g ON l.IndividualID_Patient = g.ID
		INNER JOIN dbo.Individual h ON l.IndividualID_Tech = h.ID
		INNER JOIN dbo.Prescription j ON l.PrescriptionID = j.ID
		INNER JOIN dbo.Individual m ON j.IndividualID_Doctor = m.ID
		INNER JOIN dbo.IndividualIDNumber k ON l.IndividualID_Patient = k.IndividualID
		WHERE a.OrderStatusTypeID = 1
		and d.IsFOC=0
		AND @EndDate <= DateAdd(mm,12,@BeginDate)
		AND a.DateLastModified BETWEEN @BeginDate AND @EndDate
		and l.IsActive = 1
		and SUBSTRING(l.Demographic, 5,2) not in ('32','36','31')
		and substring(l.Demographic,4,1)='A'

		If @Notes is not null
			Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 3, @Patient, @Notes
		
		----------------------------------------------
	
		SELECT a.OrderNumber,
		l.FrameBridgeSize AS bridgesizecode,
		m.LastName + ', ' + m.FirstName AS doctorname, 
		l.FrameEyeSize AS eyesizecode,
		l.FrameCode AS framecode, 
		d.FrameDescription AS framedescription,
		l.FrameColor AS framecolorcode,
		e.[Text] AS framecolordescription,
		l.Tint AS tintcode,
		f.[Text] AS tintdescription,
		l.LensType AS lenstypecode,
		b.[Text] AS lenstypedescription,
		g.LastName AS lastname,
		g.FirstName AS firstname,
		SUBSTRING(g.Demographic, 7, 1) AS sexcode,
		REPLACE(SUBSTRING(g.Demographic, 1, 3), '*', '') AS rankcode,
		l.LensMaterial AS lensmaterialcode,
		c.[Text] AS lensmaterialdescription,
		SUBSTRING(g.Demographic, 5,2) AS statuscode,
		SUBSTRING(h.FirstName, 1,1) + SUBSTRING(h.MiddleName, 1,1) + SUBSTRING(h.LastName,1,1) AS technicianinitials,
		l.FrameTempleType AS templecode,
		i.[Text] AS templedescription,
		l.NumberOfCases AS cases,
		l.UserComment1 AS comment1,
		l.UserComment2 AS comment2,
		'' AS comment3,
		'' AS comment4,
		'' AS comment5,
		'' AS comment6,
		l.Pairs AS pair,
		'' AS watermarktest1,
		'' AS watermarktext2,
		a.DateLastModified AS odereddt,
		pr.PDDistant AS pddistancecode,
		pr.PDNear AS pdnearcode,
		j.ODAdd AS odaddpowercode,
		j.OSAdd AS osaddpowercode,
		j.ODAxis AS odaxiscode,
		j.OSAxis AS osaxiscode,
		j.ODCylinder AS odcylcode,
		j.OSCylinder AS oscylcode,
		j.ODHBase AS odhbasecode,
		j.OSHBase AS oshbasecode,
		j.ODHPrism AS odhprismcode,		
		j.OSHPrism AS oshprismcode,
		l.ODSegHeight AS odsegmentheightcode,
		l.OSSegHeight AS ossegmentheightcode,
		j.ODSphere AS odspherecode,
		j.OSSphere AS osspherecode,
		j.ODVBase AS odvbasecode,
		j.OSVBase AS osvbasecode,
		j.ODVPrism AS odvprismcode,
		j.OSVPrism AS osvprismcode,
		ssn.IDNumber AS ssn, -- modified jrp 4/19/2018
		din.IDNumber AS din, -- added jrp 4/19/2018
		'' AS oddecentercode,
		'' AS osdecentercode,
		'' AS oddecenterbase,
		'' AS osdecenterbase,
		'' AS odtotaldecenter,
		'' AS ostotaldecenter,
		'' AS odtotaldecenterbase,
		'' AS ostotaldecenterbase
		FROM PatientOrderStatus a
		INNER JOIN dbo.PatientOrder l ON l.OrderNumber = a.OrderNumber
		INNER JOIN dbo.Prescription pr ON pr.ID = l.PrescriptionID
		LEFT OUTER JOIN dbo.FrameItem b ON l.LensType = b.Value
		LEFT OUTER JOIN dbo.FrameItem c ON l.LensMaterial = c.Value
		LEFT OUTER JOIN dbo.FrameItem e ON l.FrameColor = e.Value
		LEFT OUTER JOIN dbo.FrameItem f ON l.Tint = f.Value
		LEFT OUTER JOIN dbo.FrameItem i ON l.FrameTempleType = i.Value
		INNER JOIN dbo.Frame d ON l.FrameCode = d.FrameCode
		INNER JOIN dbo.Individual g ON l.IndividualID_Patient = g.ID
		INNER JOIN dbo.Individual h ON l.IndividualID_Tech = h.ID
		INNER JOIN dbo.Prescription j ON l.PrescriptionID = j.ID
		INNER JOIN dbo.Individual m ON j.IndividualID_Doctor = m.ID
	left outer join dbo.IndividualIDNumber ssn on ssn.IndividualID=l.IndividualID_Patient and ssn.IDNumberType='SSN' and ssn.IsActive=1 -- modified jrp 4/19/2018
	left outer join dbo.IndividualIDNumber din on din.IndividualID=l.IndividualID_Patient and din.IDNumberType='DIN' and din.IsActive=1	-- added jrp 4/19/2018	
		WHERE a.OrderStatusTypeID = 1
		and d.IsFOC=0
		AND @EndDate <= DateAdd(mm,12,@BeginDate)
		AND a.DateLastModified BETWEEN @BeginDate AND @EndDate
		and l.IsActive = 1
		and SUBSTRING(l.Demographic, 5,2) not in ('32','36','31')
		and substring(l.Demographic,4,1)='A'
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForLabToDispenseByLabCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForLabToDispenseByLabCode] 
@LabSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	------------ Audit MultiRead 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST
(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrderStatus t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber 
	INNER JOIN dbo.Individual b ON a.IndividualID_Patient = b.ID 
	WHERE t.IsActive = 1 
	AND t.OrderStatusTypeID = 2 
	AND a.LabSiteCode = @LabSiteCode
	AND a.IsActive = 1
	ORDER BY t.DateLastModified, a.OrderNumber                                            
      
	
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers To Dispense: ') + a.OrderNumber  + ', 
' 
	FROM  dbo.PatientOrderStatus t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber 
	INNER JOIN dbo.Individual b ON a.IndividualID_Patient = b.ID 
	WHERE t.IsActive = 1
	AND t.OrderStatusTypeID = 2 
	AND a.LabSiteCode = @LabSiteCode
	AND a.IsActive = 1
	ORDER BY t.DateLastModified, a.OrderNumber
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	-------------------------------------------
	SELECT po.OrderNumber,
	po.ClinicSiteCode,
	po.ShipToPatient, --  Added 9/17/14 for Lab Mailing to Patient - BAF
	po.ShipAddress1,
	po.ShipAddress2,
	po.ShipAddress3,
	po.ShipCity, 
	po.ShipState,
	po.ShipCountry, 
	po.ShipZipCode,
	pos.LabSiteCode,
	pos.DateLastModified, 
	pos.OrderStatusTypeID, 
	pos.StatusComment, 
	pos.ModifiedBy,
	po.FrameCode,
	po.LensType,
	po.LensMaterial, 
	i.ID as IndividualID, -- added 6/8/17 baf
	i.LastName,
	i.MiddleName,
	i.FirstName,
	po.EmailPatient 
	FROM PatientOrder po
	INNER JOIN PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	INNER JOIN Individual i on po.IndividualID_Patient = i.ID
	WHERE 
	(pos.IsActive = 1 and pos.OrderStatusTypeID = 2) and 
	po.LabSiteCode = @LabSiteCode AND
	po.IsActive = 1
	ORDER BY pos.DateLastModified, po.OrderNumber

END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForClinicToDispenseByClinicCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Change:  25 Apr 16 -- baf - Added Status 16 - Clinic Checkin NO Lab Checkout
--		19 March 18 -- baf -- Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForClinicToDispenseByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	------------ Audit MultiRead - 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
	WHERE b.OrderStatusTypeID IN (11,16)
	AND b.IsActive = 1
	AND a.IsGEyes = 0
	AND a.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers To Dispense: ') + a.OrderNumber  + ', ' 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
	WHERE b.OrderStatusTypeID IN (11,16)
	AND b.IsActive = 1
	AND a.IsGEyes = 0
	AND a.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	
	---------------------------------------------
	SELECT b.OrderNumber,
	a.ClinicSiteCode,
	b.LabSiteCode,
	b.DateLastModified AS 'DateCreated', 
	b.StatusComment, 
	b.ModifiedBy,
	a.FrameCode,
	a.LensType,
	a.LensMaterial, 
	c.ID AS IndividualID,  -- Added 6/8/2017
	c.LastName,
	c.MiddleName,
	c.FirstName,
	a.EmailPatient 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
	WHERE b.OrderStatusTypeID IN (11,16)
	AND b.IsActive = 1
	AND a.IsGEyes = 0
	AND a.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersForCheckInToClinicByClinicCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersForCheckInToClinicByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-------------- Audit MultiRead - 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder a
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
    WHERE b.OrderStatusTypeID = 7
    AND b.IsActive = 1
    AND a.ClinicSiteCode = @ClinicSiteCode
    AND a.IsGEyes = 0
    AND a.IsActive = 1                                                 
	
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers To Dispense: ') + a.OrderNumber  + ', ' 
	FROM dbo.PatientOrder a
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
    WHERE b.OrderStatusTypeID = 7
    AND b.IsActive = 1
    AND a.ClinicSiteCode = @ClinicSiteCode
    AND a.IsGEyes = 0
    AND a.IsActive = 1
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	-----------------------------------------------
	SELECT a.OrderNumber,
	a.ClinicSiteCode, 
	b.LabSiteCode,
	a.DateLastModified AS 'DateCreated',
	b.ModifiedBy,
	a.FrameCode,
	a.LensType,
	a.LensMaterial,
	CASE WHEN a.ShipToPatient = 1 THEN 'LabShipToPatient'
		WHEN a.ClinicShipToPatient = 1 THEN 'ClinicShipToPatient'
	ELSE 'Clinic Distribution' END Distribution,
	a.ShipAddress1,
	a.ShipAddress2,
	a.ShipAddress3,
	a.ShipCity,
	a.ShipState,
	a.ShipCountry, 
	a.ShipZipCode,
	c.ID AS IndividualID, -- Added 6/8/17 baf 
	c.LastName,
	c.MiddleName,
	c.FirstName,
	a.EmailPatient
    FROM dbo.PatientOrder a
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
    WHERE b.OrderStatusTypeID = 7
    AND b.IsActive = 1
    AND a.ClinicSiteCode = @ClinicSiteCode
    AND a.IsGEyes = 0
    AND a.IsActive = 1
    and a.ShipToPatient = 0
    ORDER BY b.DateLastModified
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersCountReadyForDispense]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/16/2016
-- Description:	Get Patient Orders that were rejected by the lab
--    Part of GetPatientOrderSummaryClinicBySiteCode
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersCountReadyForDispense]
	-- Add the parameters for the stored procedure here
	@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS ReadyForDispense 
    FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID IN (11,16)
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND b.OrderNumber = a.OrderNumber
	AND a.IsGEyes = 0
	AND a.IsActive = 1
	GROUP BY a.ClinicSiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersCheckInFromLabToClinic]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/16/2016
-- Description:	Get Orders Ready for Checkin from Lab to Clinic
--    Part of GetPatientOrderSummaryClinicBySiteCode
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersCheckInFromLabToClinic]
	-- Add the parameters for the stored procedure here
	@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS ReadyForCheckin 
    FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 7
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	AND NOT b.StatusComment = 'Lab Dispensed And Shipped Order To Patient'
	GROUP BY a.ClinicSiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersAvgTimeToDispense]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/16/2016
-- Description:	Get Patient Orders that were rejected by the lab
--    Part of GetPatientOrderSummaryClinicBySiteCode
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersAvgTimeToDispense]
	-- Add the parameters for the stored procedure here
	@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT @SiteCode AS ClinicSiteCode, AVG(DATEDIFF(DAY, startdate, enddate)) AS AvgDispenseDays
	from (
    SELECT a.DateLastModified AS startdate, a.OrderNumber AS ONumber
    FROM   dbo.PatientOrderStatus a
    INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber
    WHERE OrderStatusTypeID = 1
    AND b.ClinicSiteCode = @SiteCode
    AND a.OrderNumber = b.OrderNumber
    AND b.IsGEyes = 0
    AND b.IsActive = 1 and b.ModifiedBy <> 'SSIS'
	) as query1
	join (
    SELECT c.DateLastModified AS enddate, c.OrderNumber AS ONumber
    FROM   dbo.PatientOrderStatus c
    INNER JOIN dbo.PatientOrder d ON c.OrderNumber = d.OrderNumber
    WHERE c.OrderStatusTypeID = 8 
    AND d.ClinicSiteCode = @SiteCode
    AND c.OrderNumber = d.OrderNumber
    AND d.IsActive = 1 and d.ModifiedBy <> 'SSIS'
    AND d.IsGEyes = 0
	) as query2 ON query1.startdate < query2.enddate AND query1.ONumber = query2.ONumber
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersAtLabByClinicCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 August 2017
-- Description:	To retrieve a list of orders at the lab.
-- Modified:  19 Sept 2017 to include orders ship to clinic
--			19 Mar 2018 - baf - added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersAtLabByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-------------- Audit MultiRead - 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder a
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
    WHERE ((b.OrderStatusTypeID in (2,5)) or (b.OrderStatusTypeID = 7 and a.ShipToPatient = 0))
    AND b.IsActive = 1
    AND a.ClinicSiteCode = @ClinicSiteCode
    AND a.IsGEyes = 0
    AND a.IsActive = 1                                                 
	
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers To Dispense: ') + a.OrderNumber  + ', ' 
	FROM dbo.PatientOrder a
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
    WHERE ((b.OrderStatusTypeID in (2,5)) or (b.OrderStatusTypeID = 7 and a.ShipToPatient = 0))
    AND b.IsActive = 1
    AND a.ClinicSiteCode = @ClinicSiteCode
    AND a.IsGEyes = 0
    AND a.IsActive = 1
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	-----------------------------------------------
		SELECT a.OrderNumber,
		a.ClinicSiteCode, 
		b.LabSiteCode,
		a.DateLastModified AS 'DateCreated',
		b.ModifiedBy,
		a.FrameCode,
		a.LensType,
		a.LensMaterial,
		CASE WHEN a.ShipToPatient = 1 THEN 'LabShipToPatient'
			WHEN a.ClinicShipToPatient = 1 THEN 'ClinicShipToPatient'
		ELSE 'Clinic Distribution' END Distribution,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipAddress3,
		a.ShipCity,
		a.ShipState,
		a.ShipCountry, 
		a.ShipZipCode,
		c.ID AS IndividualID, -- Added 6/8/17 baf 
		c.LastName,
		c.MiddleName,
		c.FirstName,
		b.StatusComment,
		a.EmailPatient
		FROM dbo.PatientOrder a
		INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
		INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
		WHERE ((b.OrderStatusTypeID in (2,5)) or (b.OrderStatusTypeID = 7 and a.ShipToPatient = 0))
		AND b.IsActive = 1
		AND a.ClinicSiteCode = @ClinicSiteCode
		AND a.IsGEyes = 0
		AND a.IsActive = 1
		--and a.ShipToPatient = 0
		ORDER BY b.DateLastModified
    
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrderNONGEyesByOrderNumber]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		John Shear
 Create date: 31 Oct 2011
 Description:	Returns all order for a patient
 Modified: 12/15/15 - baf - Removed FOCDate
	 5/5/16 - baf - Reworked SP to use multiple orders passed
	 1/12/17 - baf - Added ClinicShipToPatient
	 2/23/17 - baf - Added DispenseComments
	 1/6/18 - baf - Added Coatings
	 3/19/18 - baf - Added EmailPatient
 =============================================*/
CREATE PROCEDURE [dbo].[GetPatientOrderNONGEyesByOrderNumber] 
	-- Add the parameters for the stored procedure here
@OrderNumber	varchar(max),
@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	---------------Audit SingleRead 10/20/14 - BAF
	Declare @ReadDate DAteTime = GetDate(),
		@Reader VarChar(200),
		@AULID int = 12,
		@PatientList VarChar(Max),
		@Notes VarChar(Max)
	
		Set @Reader = @ModifiedBy
		Set @Notes = 'Multiple Order Pull'
		Set @PatientList = @OrderNumber
		
		Exec InsertAuditMultiRead @ReadDate, @Reader,@AULID, @PatientList, @Notes
	---------------------------
	Declare @ErrorLogID int,
		@Order VarChar(16),
		@RecCnt int,
		@CurRec int = 1
		
-- First Create temporary tables to hold information passed.
	Create Table #tmpOrderNbrs (RowID int, OrderNbr VarChar(16))
	Insert into #tmpOrderNbrs
	Select * from dbo.SplitStrings(@OrderNumber,',')	

	CREATE TABLE #FullOrder
	(IndividualID_Patient int, ClinicSiteCode varchar(6), DateLastModified DateTime, Demographic varchar(8),
	 FrameBridgeSize varchar(12), FrameCode varchar(20), FrameColor varchar(3), FrameEyeSize varchar(5), FrameTempleType varchar(10), 
	 IndividualID_Tech int, IsGEyes bit, IsMultivision bit, LabSiteCode varchar(6), LocationCode varchar(10), ModifiedBy VarChar(200),
	 NumberOfCases int, ODSegHeight varchar(2), Tint varchar(15), Coatings VARCHAR(40), OrderNumber varchar(16), LensMaterial varchar(5), LensType varchar(5),  
	 OSSegHeight varchar(2), Pairs int, PrescriptionID int, ShipAddress1 varchar(200), ShipAddress2 varchar(200), ShipAddress3 varchar(200), 
	 ShipCity varchar(200), ShipState varchar(2), ShipZipCode varchar(10), ShipCountry varchar(2), ShipAddressType varchar(20), 
	 ShipToPatient bit, UserComment1 varchar(4096), UserComment2 varchar(4096), IsMonoCalculation bit, PDDistant Decimal(6,2),
	 PDNear Decimal(6,2), ODPDDistant Decimal(4,2), ODPDNear Decimal(4,2),OSPDDistant Decimal(4,2), OSPDNear Decimal(4,2),
	 PatientEmail varchar(200), PhoneNbr VarChar(15), PatientPhoneID int, OnHoldForComfirmation bit, VerifiedBy int,
	 MedProsDispense bit, PimrsDispense Bit, IsActive bit, FOCDate DateTime, LinkedID VarChar(16), StatusComment VARCHAR(500), 
	 IsComplete BIT, TechName VARCHAR(250), ClinicShipToPatient Bit, DispenseComments VarChar(45), EmailPatient BIT)
	
-- Loop through the order numbers getting full order information.
	Begin Try
		Select @RecCnt = MAX(RowID) from #tmpOrderNbrs;
		Select @Order = OrderNbr from #tmpOrderNbrs where RowID = @CurRec;
		
		While @RecCnt >= @CurRec
		Begin
			BEGIN TRANSACTION
			Insert into #FullOrder
			Select po.IndividualID_Patient, po.ClinicSiteCode, po.DateLastModified, po.Demographic, po.FrameBridgeSize, 
				po.FrameCode, po.FrameColor, po.FrameEyeSize, po.FrameTempleType, po.IndividualID_Tech, po.IsGEyes,
				po.IsMultivision, po.LabSiteCode, po.LocationCode, po.ModifiedBy, po.NumberOfCases, po.ODSegHeight,
				po.Tint, REPLACE(po.Coatings,'/',','), @Order, po.LensMaterial, po.LensType, po.OSSegHeight, po.Pairs, po.PrescriptionID,
				po.ShipAddress1, po.ShipAddress2, po.ShipAddress3, po.ShipCity, po.ShipState, po.ShipZipCode, 
				po.ShipCountry, po.ShipAddressType, po.ShipToPatient, po.UserComment1, po.UserComment2, rx.IsMonoCalculation,
				rx.PDDistant, rx.PDNear, rx.ODPDDistant, rx.ODPDNear, rx.OSPDDistant, rx.OSPDNear, po.PatientEmail,
				ipn.AreaCode + ipn.PhoneNumber as PhoneNbr, po.PatientPhoneID, po.OnholdForConfirmation, po.VerifiedBy, 
				po.MedProsDispense, po.PimrsDispense, po.IsActive, i.NextFOCDate, po.LinkedID, pos.StatusComment,
				CASE 
					WHEN EXISTS(SELECT OrderNumber FROM PatientOrderStatus t WHERE t.OrderNumber = po.OrderNumber 
						AND (t.IsActive = 1 and t.OrderStatusTypeID = 15)) THEN 0 ELSE 1
				END as IsComplete,
				tech.LastName + ', ' + tech.FirstName AS TechName, ClinicShipToPatient, DispenseComments, po.EmailPatient
			From dbo.PatientOrder po inner join dbo.Prescription rx on po.PrescriptionID = rx.ID
				INNER JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
				Inner Join dbo.IndividualPhoneNumber ipn on po.PatientPhoneID = ipn.ID
				inner join dbo.Individual i on i.ID = po.IndividualID_Patient
				INNER JOIN dbo.Individual tech ON tech.id = po.IndividualID_Tech
			where po.OrderNumber = @Order and po.IsActive = 1 AND pos.IsActive = 1

			COMMIT TRANSACTION
			SET @CurRec = @CurRec + 1;
			SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		End
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
	SELECT * FROM #FullOrder;
	
	DROP TABLE #tmpOrderNbrs;
	Drop Table #FullOrder	

END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrderByOrderNumber]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Shear
-- Create date: 31 Oct 2011
-- Description:	Returns all order for a patient
-- Modified:  1/12/17 - baf - Added ClinicShipToPatient
--	2/23/17 - baf - Added DispenseComments
--	1/5/18 - baf - Added Coatings
--	3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrderByOrderNumber] 
	-- Add the parameters for the stored procedure here
@OrderNumber	varchar(16),
@ModifiedBy		VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-------------Audit SingleRead 10/20/14 - BAF
	Declare @ReadDate DateTime = GetDate()
	Declare @PatientID int
	Declare @ReadRecID VarChar(200) = @OrderNumber
	
	Select @PatientID = IndividualID_Patient 
	From dbo.PatientOrder WHERE OrderNumber = @OrderNumber
	
	IF (@PatientID != null)
	Begin
		Exec InsertAuditSingleRead @ReadDate, @PatientID, 12, @ModifiedBy,@OrderNumber
	End	
	------------------------------------
	-- Insert statements for procedure here
	SELECT 
	a.IndividualID_Patient, 
	a.ClinicSiteCode, 
	a.DateLastModified, 
	a.Demographic, 
	a.FrameBridgeSize, 
	a.FrameCode, 
	a.FrameColor, 
	a.FrameEyeSize, 
	a.FrameTempleType,
	a.IndividualID_Tech, 
	i.LastName + ', ' + i.FirstName as TechName,
	a.IsGEyes, 
	a.IsMultivision, 
	a.LabSiteCode, 
	a.LocationCode, 
	a.ModifiedBy, 
	a.NumberOfCases, 
	a.ODSegHeight, 
	a.Tint, 
	REPLACE(a.Coatings,'/',',')AS Coatings, -- Added 1/5/18
	a.OrderNumber, 
	a.LensMaterial, 
	a.LensType, 
	a.OSSegHeight, 
	a.Pairs, 
	a.PrescriptionID, 
	a.ShipAddress1, 
	a.ShipAddress2,
	a.ShipAddress3, 
	a.ShipCity, 
	a.ShipState, 
	a.ShipZipCode,
	a.ShipAddressType, 
	a.ShipCountry,  
	a.ShipToPatient, 
	a.UserComment1, 
	a.UserComment2, 
	pr.IsMonoCalculation,
	pr.PDDistant, 
	pr.PDNear, 
	pr.ODPDDistant, 
	pr.ODPDNear, 
	pr.OSPDDistant, 
	pr.OSPDNear, 
	a.PatientEmail, 
	b.AreaCode + b.PhoneNumber as PatientPhoneNumber,
	--'(' + b.AreaCode + ') ' + b.PhoneNumber AS PatientPhoneNumber,
	a.PatientPhoneID,  
	a.OnholdForConfirmation, 
	a.VerifiedBy,
	a.MedProsDispense,
	a.PimrsDispense, 
	a.IsActive, 
	f.FrameDescription, -- Added for G-Eyes DB 02 Dec 2013
	fitem1.[Text] AS 'LensTint', -- Added for G-Eyes DB 02 Dec 2013
	fitem2.[Text] AS 'LensTypeLong', -- Added for G-Eyes DB 02 Dec 2013
	CASE WHEN a.Coatings = 'AR' THEN 'Anti-Refl'
		WHEN a.Coatings = 'UV' THEN 'UV-400'
		WHEN a.Coatings = 'AR/UV' THEN 'Anti-Refl,UV-400'
		WHEN a.Coatings = 'UV/AR' THEN 'UV-400,Anti-Refl'
	END LensCoating,
--	i.NextFOCDate AS FOCDate,
	LinkedID,
	CASE 
	WHEN EXISTS(SELECT OrderNumber FROM PatientOrderStatus t WHERE t.OrderNumber = a.OrderNumber and (t.IsActive = 1 and t.OrderStatusTypeID = 15)) THEN 0 ELSE 1
	END as IsComplete,
	pos.StatusComment,
	a.ClinicShipToPatient,
	--a.DispenseComments,
	Case when (a.GroupName is not null) or a.Groupname <> '' then	
		a.DispenseComments + ' (' + a.GroupName + ')'
	else a.DispenseComments end DispenseComments,
	a.EmailPatient		-- Added 19 Mar 18 - baf
	FROM dbo.PatientOrder a 
	INNER JOIN dbo.PatientOrderStatus pos ON a.OrderNumber = pos.OrderNumber
	INNER JOIN dbo.Individual i ON i.ID = a.IndividualID_Tech
	INNER JOIN dbo.Prescription pr ON pr.ID = a.PrescriptionID
	INNER JOIN dbo.IndividualPhoneNumber b ON a.PatientPhoneID = b.ID
	INNER JOIN dbo.Frame f ON a.FrameCode = f.FrameCode -- Added for G-Eyes DB 02 Dec 2013
	INNER JOIN dbo.FrameItem fitem1 ON fitem1.Value = a.Tint -- Added for G-Eyes DB 02 Dec 2013
	INNER JOIN dbo.FrameItem fitem2 ON fitem2.Value = a.LensType -- Added for G-Eyes DB 02 Dec 2013
	WHERE a.OrderNumber = @OrderNumber and fitem1.TypeEntry = 'TINT' AND fitem1.IsActive = 1
		and a.IsActive = 1 AND pos.IsActive = 1
		AND pos.OrderStatusTypeID NOT IN (14) AND pos.IsActive = 1 -- 1/4/16 baf
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrderByIndividualIdNonGEyes]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		Kevin Bush
 Create date: 3 Jun 14
 Description:	Get an individuals order info without GEyes data.
 Modified:  7/22/2015 - baf - Changed GEyes1 site code to 999999
           11/13/2015 - jrp - Removed SiteCode param and references
 	12/8/15 - kb/baf - Removed fields for COA1 changed order by Claus,
		Added Technician Name
   12/15/15 - baf - Removed FOCDate, added StatusComment
 	1/4/16 - baf - Check/Remove Clinic Cancelled Orders
	1/12/17 - baf - Added ClinicShipToPatient
	2/23/17 - baf - Added DispenseComments 
	1/5/18 - baf - Added Coatings
	3/19/18 - baf - Added EmailPatient
 =============================================*/
CREATE PROCEDURE [dbo].[GetPatientOrderByIndividualIdNonGEyes] 
	-- Add the parameters for the stored procedure here
@IndividualID	INT,
@ModifiedBy		VarChar(200)
-- ,@SiteCode		VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	------------- Audit Multi Read - 10/17/14 - BAF
	Declare @ReadDate	DateTime	= GetDate()
	Declare @Patient	VarChar(20) = Cast(@IndividualID as VarChar(20))
	Declare	@Notes		VarChar(Max)
	
	Select @Notes = Coalesce (@Notes, 'Order Numbers: ') + OrderNumber + ', '
	FROM dbo.PatientOrder a
	--INNER JOIN dbo.IndividualPhoneNumber b ON a.PatientPhoneID = b.ID
	WHERE IndividualID_Patient = @IndividualID 
	
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @Patient, @Notes
	-----------------

    -- Insert statements for procedure here
	SELECT 
	a.IndividualID_Patient, 
	a.ClinicSiteCode, 
	a.DateLastModified, 
	a.Demographic, 
	a.FrameBridgeSize, 
	a.FrameCode, 
	a.FrameColor, 
	a.FrameEyeSize, 
	a.FrameTempleType,
	a.IndividualID_Tech, 
	i.LastName + ', ' + i.FirstName as TechName,
	a.IsGEyes, 
	a.IsMultivision, 
	a.LabSiteCode, 
	a.LocationCode, 
	a.ModifiedBy, 
	a.NumberOfCases, 
	a.ODSegHeight, 
	a.Tint, 
	REPLACE(a.Coatings,'/',',') AS Coatings, -- Added 1/5/18
	a.OrderNumber, 
	a.LensMaterial, 
	a.LensType, 
	a.OSSegHeight, 
	a.Pairs, 
	a.PrescriptionID, 
	a.ShipAddress1, 
	a.ShipAddress2,
	a.ShipAddress3, 
	a.ShipCity, 
	a.ShipState, 
	a.ShipZipCode, 
	a.ShipAddressType, 
	a.ShipCountry, 
	a.ShipToPatient, 
	a.UserComment1, 
	a.UserComment2,
	c.IsMonoCalculation,
	c.PDDistant,
	c.PDNear, 
	c.ODPDDistant,
	c.ODPDNear, 
	c.OSPDDistant,
	c.OSPDNear,
	--a.PatientEmail, 
	--b.AreaCode + b.PhoneNumber as PatientPhoneNumber,
	--'(' + b.AreaCode + ') ' + b.PhoneNumber AS PatientPhoneNumber,
	--a.PatientPhoneID,  
	--a.OnholdForConfirmation, 
	--a.VerifiedBy,
	--a.MedProsDispense,
	--a.PimrsDispense, 
	a.IsActive,
	--a.FOCDate, 
	a.LinkedID,
	CASE 
	WHEN EXISTS(SELECT OrderNumber FROM PatientOrderStatus t WHERE t.OrderNumber = a.OrderNumber and (t.IsActive = 1 and t.OrderStatusTypeID = 15)) THEN 0 ELSE 1
	END as IsComplete,
	pos.StatusComment,
	a.ClinicShipToPatient,
	a.DispenseComments,
	a.EmailPatient -- Added 3/19/18 baf
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus pos ON a.OrderNumber = pos.OrderNumber
	INNER JOIN dbo.Prescription c ON a.PrescriptionID = c.ID
	INNER JOIN dbo.Individual i on a.IndividualID_Tech = i.ID
	--INNER JOIN dbo.IndividualPhoneNumber b ON a.PatientPhoneID = b.ID
	WHERE a.IndividualID_Patient = @IndividualID
--		AND ((a.ClinicSiteCode = @SiteCode) OR @SiteCode IN ('ADM001', '999999' ))
		and a.IsActive = 1 AND pos.IsActive = 1
		AND pos.OrderStatusTypeID NOT IN (14) AND pos.IsActive = 1 -- 1/4/16 baf
	ORDER BY DateLastModified Desc


END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientFacingOrders]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: Jan 27 2017
-- Description:	Retrieves order information for last 2 years
--		for a specific patient
-- Modified:  1/4/18 - baf - Added Coatings
--		3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientFacingOrders]
	-- Add the parameters for the stored procedure here
	@Patient INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

Select po.*, pos.LabSiteCode, pos.OrderStatusTypeID, pos.StatusComment, pos.DateLastModified
From 
(
Select ClinicSiteCode AS Clinic, po.DateLastModified AS OrderDate, OrderNumber, FrameCode, 
	FrameEyeSize, FrameBridgeSize, FrameTempleType,
	fc.[Text] AS FrameColor,fl.[Text] as LensType, Tint, Coatings, po.EmailPatient
FROM dbo.PatientOrder po 
	LEFT JOIN dbo.FrameItem fc ON po.FrameColor = fc.Value
	LEFT JOIN dbo.FrameItem fl ON po.LensType = fl.Value
where	fc.TypeEntry = 'Color' AND fl.TypeEntry = 'Lens_Type' and
	IndividualID_Patient = @Patient and
	CAST(po.DateLastModified as Date) >= DATEADD(yy,-2,Cast(GetDate() as Date))
) po	
Inner Join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
Left Join dbo.OrderStatusType ost on ost.ID = pos.OrderStatusTypeID
Where pos.IsActive = 1
ORDER BY pos.DateLastModified DESC, OrderNumber Asc
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientEmails]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 28 March 2018
-- Description:	Retrieve all order emails for a patient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientEmails]
	@PatientID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.OrderEmail WHERE PatientID = @PatientID
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrderEmail]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 5 Apr 2018
-- Description:	Retrieves a patient's email by order number
-- =============================================
CREATE PROCEDURE [dbo].[GetOrderEmail]
	-- Add the parameters for the stored procedure here
	@OrderNbr VARCHAR(16)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.OrderEmail WHERE OrderNumber = @OrderNbr
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrderDetailReport]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  1/4/2018 - baf - added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetOrderDetailReport]
	-- Add the parameters for the stored procedure here
	@FromDate Datetime,
	@ToDate Datetime,
	@SiteCode varchar(6) = Null,
	@Status VarChar(2) = Null,
	@Priority VarChar(1) = Null,
	@ModifiedBy VarChar(200)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	---------- Audit Multi Read - 10/23/14 - BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'Patient IDs: ') + Cast(po.IndividualID_Patient as VarChar(25)) + ', '
	from dbo.Prescription pr inner join
		dbo.PatientOrder po on po.PrescriptionID = pr.ID
		inner join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
		inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
		inner join dbo.IndividualIDNumber inIDNbr on po.IndividualID_Patient = inIDNbr.IndividualID
		inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
		inner join dbo.SiteAddress sa on sc.SiteCode = sa.SiteCode
		inner join dbo.SiteCode csc on csc.SiteCode = po.ClinicSiteCode
	where pos.OrderStatusTypeID = 1 
		and pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)  
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and po.ClinicSiteCode = @SiteCode 
		and sa.AddressType = 'SITE'
		and inIDNbr.IsActive = 1
	
	Select @Notes = Coalesce(@Notes, 'Order Detail Rpt OrderNumbers: ') + po.OrderNumber + ', '
	from dbo.Prescription pr inner join
		dbo.PatientOrder po on po.PrescriptionID = pr.ID
		inner join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
		inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
		inner join dbo.IndividualIDNumber inIDNbr on po.IndividualID_Patient = inIDNbr.IndividualID
		inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
		inner join dbo.SiteAddress sa on sc.SiteCode = sa.SiteCode
		inner join dbo.SiteCode csc on csc.SiteCode = po.ClinicSiteCode
	where pos.OrderStatusTypeID = 1 
		and pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)  
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and po.ClinicSiteCode = @SiteCode 
		and sa.AddressType = 'SITE'
		and inIDNbr.IsActive = 1
		
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	----------------------------------------------

select distinct po.OrderNumber, SUBSTRING(po.demographic,5,2) as StatusCode,
	SUBSTRING(po.demographic,8,1) as PriorityCode, po.ClinicSiteCode, 
	csc.SiteName as ClinicName, po.LabSiteCode, sc.SiteName, po.FrameBridgeSize as dbl,
	ind.FirstName, ind.LastName, SUBSTRING(ind.MiddleName,1,1) as MI, pr.OSHPrism, pr.ODHPrism,
	RIGHT(inIDNbr.IDNumber,4) as LastFour, pr.ODSphere, pr.ODCylinder, pr.OSVPrism, pr.ODVPrism,
	pr.ODAxis, pr.ODAdd, po.ODSegHeight, pr.OSSphere, pr.OSCylinder, pr.OSAxis, pr.OSHBase, pr.ODHBase,
	pr.ODVBase, pr.OSVBase, pr.OSAdd, po.OSSegHeight, FrameCode, FrameColor, FrameTempleType, 
	FrameEyeSize, LensMaterial, LensType,Tint, Coatings, Pairs, NumberOfCases, pos.DateLastModified as OrderDate,
	pr.IsMonoCalculation, pr.ODPDDistant, pr.ODPDNear, pr.OSPDDistant, pr.OSPDNear, pr.PDDistant, pr.PDNear
from dbo.Prescription pr inner join
	dbo.PatientOrder po on po.PrescriptionID = pr.ID
	inner join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
	inner join dbo.IndividualIDNumber inIDNbr on po.IndividualID_Patient = inIDNbr.IndividualID
	inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
	inner join dbo.SiteAddress sa on sc.SiteCode = sa.SiteCode
	inner join dbo.SiteCode csc on csc.SiteCode = po.ClinicSiteCode
where pos.OrderStatusTypeID = 1 
	and pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)  
	and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
	and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
	and po.ClinicSiteCode = @SiteCode 
	and sa.AddressType = 'SITE'
	and inIDNbr.IsActive = 1
Order by LabSiteCode, LastName, LastFour, OrderDate desc
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetNostraFileData]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
 Author:		Kevin Bush
 Create date: 9 Apr 14
 Description:	This is a similar sp to Get711DataByLabCode but will have a few more fields.
				This sp was replicated due to potential issues with SSRS and the regeneration of reports
				when changes are made to the sp.
				This sp gets all the necessary data to build a pipe delimited file to ship off to NOSTRA.
 Modified:  KDB - 3/12/15 - PD Changes
			JRP - 4/28/15 - Removed extraneous function calls
		   JRP - 6/29/15 - Fixed UV Translation	
      		BAF - 10/5/2015 - Stripping down Audit based on number of orders to speed up query process, and
						rework ID Number piece. 
			BAF - 10/16/2015 - Removed check for IDNumberType = 'SSN'
			BAF - 5/27/16 - Added TBI Tints
			BAF - 1/10/18 - Added Coating
-- ============================================= */
CREATE PROCEDURE [dbo].[GetNostraFileData]
@SiteCode	VARCHAR(6) = Null,
@OrderNbr Varchar(16) = Null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @orderPriority varchar(1),
		@RecCnt INT,
		@CurRec INT = 1,
		@MaxDate DATETIME,
		@MinDate DateTime
	
	CREATE TABLE #tmpLMSOrders
	(RowID INT, OrderNumber VARCHAR(MAX), DateLastModified DATETIME, PatientID INT, IDNbr VARCHAR(12))
	
	IF @OrderNbr IS NULL
	BEGIN
		INSERT INTO #tmpLMSOrders(OrderNumber, DateLastModified, PatientID,IDNbr)
		SELECT po.OrderNumber, pos.DateLastModified,po.IndividualID_Patient, 
			CASE WHEN xx.IDNumber IS NULL THEN idn.IDNumber ELSE xx.IDNumber END PatientID
		FROM dbo.PatientOrder po 
			INNER JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
			INNER JOIN dbo.IndividualIDNumber idn ON idn.IndividualID = po.IndividualID_Patient
			LEft JOIN (SELECT IDNumber, IndividualID FROM dbo.IndividualIDNumber  WHERE
				IDNumberType = 'DIN')xx ON xx.IndividualID = po.IndividualID_Patient
		WHERE po.IsActive = 1 AND pos.IsActive = 1 AND
			pos.OrderStatusTypeID in (1,6,4,9,10) AND po.ModifiedBy <> 'SSIS' 
			AND (@SiteCode is Null or pos.LabSiteCode = @SiteCode)
			AND idn.IsActive = 1 --AND idn.IDNumberType = 'SSN'
			
	END
	ELSE
	BEGIN
		INSERT INTO #tmpLMSOrders(OrderNumber, DateLastModified, PatientID,IDNbr)
		SELECT po.OrderNumber, pos.DateLastModified,po.IndividualID_Patient, 
			CASE WHEN xx.IDNumber IS NULL THEN idn.IDNumber ELSE xx.IDNumber END PatientID
		FROM dbo.PatientOrder po 
			INNER JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
			INNER JOIN dbo.IndividualIDNumber idn ON idn.IndividualID = po.IndividualID_Patient
			LEft JOIN (SELECT IDNumber, IndividualID FROM dbo.IndividualIDNumber  WHERE
				IDNumberType = 'DIN')xx ON xx.IndividualID = po.IndividualID_Patient
		WHERE po.IsActive = 1 AND pos.IsActive = 1 AND
			pos.OrderStatusTypeID in (1,6,4,9,10) AND po.ModifiedBy <> 'SSIS' 
			AND po.OrderNumber = @OrderNbr
			AND idn.IsActive = 1 --AND idn.IDNumberType = 'SSN'
	End
	

	SELECT @MaxDate = MAX(DateLastModified) FROM #tmpLMSOrders;
	SELECT @MinDate = MIN(DateLastModified) FROM #tmpLMSOrders;
	SELECT @RecCnt = COUNT(*) FROM #tmpLMSOrders;
	
	------ Audit MultiRead Routine -- 10/16/14 - BAF
	---  Multiple Records for one patient
	Declare @ReadDate		DateTime = GetDate()
	Declare @ModifiedBy		VarChar(200) = 'WebService'
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)

	
	IF @RecCnt > 100
		BEGIN
			SET @PatientList = 'LMS Patients:  List > 100 records for this SiteCode: ' + @SiteCode
				+ ' OrderStatus Dates from: ' + Cast(@MaxDate as VarChar(60)) + ' TO ' + Cast(@MinDate as VarChar(60))
			SET @Notes = 'LMS OrderNumbers: Count = ' + Cast(@RecCnt as VarChar (4)) + ' for SiteCode: ' + @SiteCode 
					+ '  OrderStatus dates from : ' + Cast(@MaxDate as VarChar(60)) + ' TO ' + Cast(@MinDate as VarChar(60))
		END
	ELSE
		BEGIN
			Select @PatientList = Coalesce(@PatientList, 'PatientIDs: ') + CAST(PatientID as VarChar(20)) + ', '
			FROM  #tmpLMSOrders
			
			Select @Notes = Coalesce (@Notes, 'OrderNbrs: ') + OrderNumber + ','
			From #tmpLMSOrders			
		END
		
	IF @Notes is not null 
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes	
	----------------------------
	
--SELECT DISTINCT OrderNumber, DocumentNumber, Demographic, SexCode, RankCode, StatusCode, OrderPriority, BOS, BillingCode,
--	techinitials, LensMaterial, LensStatus, LensType, RawLensType, NumberOfCases, Pairs, LmsComment, FrameCode,
--	FrameColor, FrameEyeSize, FrameBridgeSize, FrameTempleType, ODPDDistant, OSPDDistant, PDDistant, ODPDNear, OSPDNear,
--	PDNear,	ClinicSiteCode, clinicname,DINID,
--	CASE WHEN DINID IS NULL THEN SSNID ELSE DINID END IndividualID_Patient
	
--FROM (
	SELECT distinct
		a.OrderNumber,
		Left(a.OrderNumber,13) as DocumentNumber,
		--a.ClinicSiteCode + convert(varchar(9), a.OrderNumber) as DocumentNumber,
		a.Demographic,
		SUBSTRING(a.Demographic, 7, 1) AS sexcode,
		SUBSTRING(a.Demographic, 1, 3) AS rankcode,
		SUBSTRING(a.Demographic, 5,2) AS statuscode,
		--SUBSTRING(a.Demographic, 8, 1) AS orderpriority,
		case 
			when upper(RIGHT(a.Demographic, 1)) = 'F' then 'FOC'
			when upper(RIGHT(a.Demographic, 1)) = 'P' then 'DWN PILOT'
			when upper(RIGHT(a.Demographic, 1)) = 'R' then 'READINESS'
			when upper(RIGHT(a.Demographic, 1)) = 'T' then 'TRAINEE'
			when upper(RIGHT(a.Demographic, 1)) = 'V' then 'VIP'
			when upper(RIGHT(a.Demographic, 1)) = 'W' then 'W'
			when upper(RIGHT(a.Demographic, 1)) = 'S' then 'STANDARD'
			else ''
		end as orderpriority,
		SUBSTRING(a.Demographic, 4, 1) AS bos,
		bc.Value as BillingCode,
		SUBSTRING(m.FirstName, 1,1) + SUBSTRING(m.MiddleName, 1,1) + SUBSTRING(m.LastName,1,1) AS techinitials,
		
		-- If lenstype is and single vision or half eye lens, then 
		--case 
		--	when upper(a.LensType) = 'SVD' or UPPER(a.LensType) = 'SVN' or UPPER(a.LensType) = 'SVAL' or upper(a.LensType) = 'HLF' 
		--	then 
		--		case 
		--			when UPPER(a.LensType) = 'SVD'
		--				then 
		--					convert(varchar(6), PDDistant)
		--				else -- It is a near lens so use the near measurement with a M at the end
		--					convert(varchar(6), PDNear)
		--		end
		--	else -- It is a multivision lens and uses both measurements and the distant measurement has a B after it
		--		convert(varchar(6), a.PDDistant) + 'B ' + convert(varchar(6), a.PDNear)
		--end as PupillaryDistant,
		
		case 
			when a.LensMaterial = 'PLAS' then 'CPLS'
			when a.LensMaterial = 'POLY' then 'CPLY'
			when a.LensMaterial = 'HI' then 'HPLS'
			when a.LensMaterial = 'TRAN' then 'TRNG'
			when a.LensMaterial = 'LSR' then 'LSR'
			when a.LensMaterial = 'GLAS' then 'GLAS'
			else ''
		end as LensMaterial,
		
		'COMP' as LensStatus,
		--a.Tint,
		
		case
			when a.LensType = 'BI' then '28'
			when a.LensType = 'BIW' then '35'
			when a.LensType = 'TRI' then '7X28'
			when a.LensType = 'TRIW' then '8X35'
			when a.LensType = 'BIAL' then 'STAL'
			when a.LensType = 'SVD' then 'SV'
			when a.LensType = 'SVN' then 'SV'
			when a.LensType = 'QUAD' then 'QUAD'
			when a.LensType = 'DD' then 'DD'
			when a.LensType = 'SVAL' then 'SVAL'
			else ''
		end as LensType,
		a.LensType as RawLensType,
		case 
			when a.ODSegHeight = '3' then '3D'
			when a.ODSegHeight = '4' then '4D'  
			else a.ODSegHeight end as ODSegHeight,
		case 
			when a.OSSegHeight = '3' then '3D' 
			when a.OSSegHeight = '4' then '4D'
			else a.OSSegHeight end as OSSegHeight,
		a.NumberOfCases,
		a.Pairs,
		CASE WHEN UPPER(a.FrameCode) = 'HLF' THEN 'HALF EYE FRAME' ELSE '' END AS LmsComment,
		a.FrameCode,
		a.FrameColor,
		a.FrameEyeSize,
		a.FrameBridgeSize,
		case when upper(right(a.FrameTempleType, 1)) = 'C' then a.FrameTempleType + 'Z' else a.FrameTempleType end as FrameTempleType,
		
		g.ODPDDistant,
		g.OSPDDistant,
		g.PDDistant,
		g.ODPDNear,
		g.OSPDNear,
		g.PDNear,
		
		/*
		a.ODPDDistant,
		a.OSPDDistant,
		a.PDDistant,
		a.ODPDNear,
		a.OSPDNear,
		a.PDNear,
		*/
		
		a.ClinicSiteCode,
		c.SiteName AS clinicname,
		e.Address1 AS clinicaddress1,
		e.Address2 AS clinicaddress2,
		e.Address3 as clinicaddress3,
		e.City AS cliniccity,
		e.Country AS cliniccountry,
		e.State AS clinicstate,
		e.ZipCode AS cliniczipcode,
		b.LabSiteCode,
		d.SiteName AS labname,
		f.Address1 AS labaddress1,
		f.Address2 AS labaddress2,
		f.Address3 as labaddress3,
		f.City AS labcity,
		f.Country AS labcountry,
		f.State AS labstate,
		f.ZipCode AS labzipcode,
		case when a.ShipToPatient = 1 then 'Y' else 'N' end as ShipToPatient,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipAddress3,
		a.ShipCity,
		a.ShipState,
		case when LEN(a.ShipZipCode) > 5 then LEFT(a.ShipZipCode,5) end BaseZip,
		case when LEN(a.shipZipCode) > 5 then RIGHT(a.ShipZipCode,4) end ZipPlus,
		a.ShipZipCode,
		a.ShipAddressType,
		a.LocationCode,
		replace(a.UserComment1,'|','_') as UserComment1,
		replace(a.UserComment2,'|','_') as UserComment2,
		'' as UserComment3,
		'' as UserComment4,
		'' as UserComment5,
		'' as UserComment6,
        a.IsGEyes,
        a.IsMultivision,
        a.VerifiedBy,
        a.PatientEmail,
        a.DateLastModified AS dateordercreated,
        a.OSDistantDecenter as osdistantdecenter,
        a.ODDistantDecenter as oddistantdecenter,
        a.OSNearDecenter as osneardecenter,
        a.ODNearDecenter as odneardecenter,
        a.ODBase as odbase,
        a.OSBase as osbase,
        g.ODAdd,
        g.ODAxis,
        g.OSAdd,
        g.OSAxis,
        g.ODCylinder,
        --case when upper(g.ODCylinder) <> 'SPHERE' then g.ODCylinder end as ODCylinder,
		g.OSCylinder,
		--case when UPPER(g.OSCylinder) <> 'SHPERE' then g.OSCylinder end as OSCylinder,
        
        -- If Prism is blank then base is blank
        case when g.ODHPrism = 0.00 then '' else CONVERT(varchar(6), g.ODHPrism) end as ODHPrism, 
        case when g.ODHPrism = 0.00 then '' else g.ODHBase end as ODHBase,
        
        case when g.ODVPrism = 0.00 then '' else convert(varchar(6), g.ODVPrism) end as ODVPrism,
        case when g.ODVPrism = 0.00 then '' else g.ODVBase end as ODVBase,
        
        case when g.OSHPrism = 0.00 then '' else convert(varchar(6), g.OSHPrism) end as OSHPrism,
        case when g.OSHPrism = 0.00 then '' else g.OSHBase end as OSHBase,
        
        case when g.OSVPrism = 0.00 then '' else convert(varchar(6), g.OSVPrism) end as OSVPrism,
        case when g.OSVPrism = 0.00 then '' else g.OSVBase end as OSVBase,
        
        -- THIS APPLYS TO BOTH OD AND OS
        -- If ODHPrism is blank then add ODV prism and base
        -- Else add ODH prism and base, add a space, add ODV prism and base
        case when ODHPrism = 0.00
			then cast(g.ODVPrism as varchar) + g.ODVBase
			else cast(g.ODHPrism as varchar) + g.ODHBase
			+ ' ' 
			+ cast(g.ODVPrism as varchar) + g.ODVBase end as ODPrism,
		
		case when OSHPrism = 0.00
			then cast(g.OSVPrism as varchar) + g.OSVBase
			else cast(g.OSHPrism as varchar) + g.OSHBase + ' ' + cast(g.OSVPrism as varchar) + g.OSVBase end as OSPrism,
        
        g.ODSphere,
        g.OSSphere,
        
        h.ODCorrectedAcuity,
        h.ODOSCorrectedAcuity,
        h.ODOSUncorrectedAcuity,
        h.ODUncorrectedAcuity,
        h.OSCorrectedAcuity,
        h.OSUncorrectedAcuity,
        h.examdate,
        i.FirstName,
        i.MiddleName,
        i.LastName,
        j.LastName + ', ' + j.FirstName AS doctor,
        tmp.IDNbr AS patientidnumber,
       -- k.IDNumber AS DINID,
		--idnSSN.IDNumber AS SSNID,
         l.AreaCode + l.PhoneNumber AS patientphonenumber,
        n.[Text] as BOSDesc,
        o.[text] as StatDesc,
        p.[Text] as OrdPriDesc,
   --     case
			--when q.[Value] = '15' then 'N-15 Drk Grey'
			--when q.[Value] = '31' then 'N-31 Med Grey'
			--when q.[Value] = '40' then 'Nom. 60% Trans'
			--when q.[Value] = 'AR' then 'Anti-Refl'
			--when q.[Value] = 'CL' then 'Clear'
			--when q.[Value] = '51' then 'S-1 Lt Pink'
			--when q.[Value] = '52' then 'S-2 Med Pink'
			--when q.[Value] = 'UV' then 'UV-400'
   --     end TintDesc,
   
   /*
		If ProcType = TNT
		  @tint = vStarCode
		else
		  @tint = srtsCode

		VStarCode	SrtsCode	ProcType
		AR			AR			ARC
		N15			15			TNT
		N31			31			TNT
		N60			40			TNT
		SL1			S1			TNT
		SL2			S2			TNT
		UV4			UV			UVC
		SL3			S3			TNT
   */
		case 
--			when q.Value = 'AR' then 'AR'
			when q.Value = '15' then 'N15'
			when q.Value = '31' then 'N31'
			when q.Value = '40' then 'N60'
			when q.Value = 'S1' then 'SL1'
			when q.Value = 'S2' then 'SL2'
--			when q.Value = 'UV' then 'UV4'
			when q.Value = 'S3' then 'SL3'
		--	when q.Value = 'CL' then 'CLR'
			-- specialty tints for TBI
			when q.Value = '60' then 'N40' -- Gray Nominal 40% Transmission
			when q.Value = 'P8' then 'PK85' -- Pink 85%
			when q.Value = 'P6' then 'PK60' -- Pink 60%
			when q.Value = 'A8' then 'AM80' -- Amber 80%
			when q.Value = 'A6' then 'AM60' -- Amber 60%
			when q.Value = 'B8' then 'BL85' -- Blue 85%
			when q.Value = 'B6' then 'BL65' -- Blue 65%
			when q.Value = 'FL' then 'FL41' -- Rose 50%
			else ''
		end Tint,		
		--q.Text as TintDesc,
        r.[Text] as ColorDesc,
        s.[Text] as MaterialDesc,
        t.[text] as LensDesc,
        a.ONBarCode,
        --k.IDNumber as IndividualID_Patient,
        tmp.IDNbr AS IndividualID_Patient,
        --idnSSN.IDNumber AS SSNID,
        ia.UIC as Unit,
        replace(a.DispenseComments,'|','_') as DispenseComments,
        CASE WHEN a.Coatings = 'AR' THEN 'AR' -- Added 1/10/18
			WHEN a.coatings = 'UV' THEN 'UV4'
			WHEN a.Coatings = 'AR/UV' THEN 'ARU'
			WHEN a.Coatings = 'UV/AR' THEN 'ARU'
			else ''
		END Coating
    FROM dbo.PatientOrder a 
    INNER JOIN #tmpLMSOrders tmp ON a.OrderNumber = tmp.OrderNumber
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
   -- INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    LEFT OUTER JOIN dbo.IndividualAddress ia ON a.IndividualID_Patient = ia.ID and ia.UIC = 'UNIT'
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.LookupTable bc on SUBSTRING(a.Demographic, 4, 1) + SUBSTRING(a.Demographic, 5, 2) = bc.[Text]
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	--LEFT  JOIN dbo.IndividualIDNumber idnSSN ON idnSSN.IndividualID = a.IndividualID_Patient
	WHERE a.IsActive = 1 and a.ModifiedBy <> 'SSIS'
	AND b.IsActive = 1
	--AND k.IsActive = 1 and k.IDNumberType = 'DIN'
	--AND idnSSN.IsActive = 1 AND idnSSN.IDNumberType = 'SSN'
	AND b.OrderStatusTypeID in (1,6,4,9,10)
	--AND (b.OrderStatusTypeID = 1 OR b.OrderStatusTypeID = 6 OR b.OrderStatusTypeID = 4 
	--OR b.OrderStatusTypeID = 9 OR b.OrderStatusTypeID = 10)
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND bc.Code = 'VStarBillCode'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	AND (@SiteCode is Null or a.LabSiteCode = @SiteCode)
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
--) z
	ORDER BY OrderNumber

DROP TABLE #tmpLMSOrders;

END
GO
/****** Object:  StoredProcedure [dbo].[GetNonStandardFrames]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/26/2013
-- Description:	This stored procedure retrieves all data necessary
--		to complete the NonStandard Frame portion of the 
--		Summary for All Orders Processed (Rpt 54).
-- =============================================
CREATE PROCEDURE [dbo].[GetNonStandardFrames]
	-- Add the parameters for the stored procedure here
	@FromDate Datetime,
	@ToDate Datetime,
	@LabSiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select LabSiteCode, FrameCode, SUM(NAD) as TNAD, SUM(NRet) as TNRET, 
	SUM(MAD) as TMAD, SUM(MRet) as TMRet, SUM(AAD) as TAAD, SUM(ARet) as TARet,
	SUM(FAD) as TFAD, sum(FRet) as TFRet, SUM(PDRL) as TPDRL, SUM(FmrPOW) as TPOW,
	SUM(CAD) as TCAD, SUM(PAD) as TPAD, SUM(BAD) as TBAD, SUM(FRes) as TFRES, 
	SUM(ARes) as TARes, SUM(ANG) as TANG, sum(FNG) as TFNG,SUM(NRes) as TNRes,
	SUM(NDepAD) as TNDep, SUM(MRes) as TMRes, SUM(DodRemCONUS) as TDRCONUS, 
	SUM(VABen) as TVABen, SUM(IMET) as TIMET, SUM(Other) as TOther
from
(
	select LabSiteCode, FrameCode,
		case when a.bos = 'N' and statuscode in (11,14) then Pairs else 0 end NAD,
		case when a.bos = 'N' and statuscode = 31 then Pairs else 0 end NRet,
		case when a.bos = 'M' and statuscode = 11 then Pairs else 0 end MAD,
		case when a.bos = 'M' and statuscode = 31 then Pairs else 0 end MRet,
		case when a.bos = 'A' and statuscode in (11,14) then Pairs else 0 end AAD,
		case when a.bos = 'A' and statuscode = 31 then Pairs else 0 end ARet,
		case when a.bos = 'F' and statuscode in (11,14) then Pairs else 0 end FAD,
		case when a.bos = 'F' and statuscode = 31 then Pairs else 0 end FRet,
		case when statuscode = 32 then Pairs else 0 end PDRL,
		case when statuscode = 36 then Pairs else 0 end FmrPOW,
		case when a.bos = 'C' and statuscode = 11 then Pairs else 0 end CAD,
		case when a.bos = 'P' and statuscode = 11 then Pairs else 0 end PAD,
		case when a.bos = 'B' and statuscode = 11 then Pairs else 0 end BAD,
		case when a.bos = 'F' and statuscode = 12 then Pairs else 0 end FRes,
		case when a.bos = 'A' and statuscode = 12 then Pairs else 0 end ARes,
		case when a.bos = 'A' and statuscode = 15 then Pairs else 0 end ANG,
		case when a.bos = 'F' and statuscode = 15 then Pairs else 0 end FNG,
		case when a.bos = 'N' and statuscode = 12 then Pairs else 0 end NRes,
		case when a.bos = 'N' and statuscode = 41 then Pairs else 0 end NDepAD,
		case when a.bos = 'M' and statuscode = 12 then Pairs else 0 end MRes,
		case when a.bos = 'K' and statuscode = 55 then Pairs else 0 end DodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 then Pairs else 0 end VABen,
		case when a.bos = 'K' and statuscode = 71 then Pairs else 0 end IMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) then Pairs else 0 end Other
	from
	(
		select pos.OrderStatusTypeID, po.ClinicSiteCode, po.LabSiteCode,
			REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(i.Demographic, 5,2) AS statuscode,
			SUBSTRING(i.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(i.Demographic, 4, 1) AS bos, po.FrameCode, po.LensType,
			po.Pairs, f.FrameDescription, f.FrameNotes,
			case when f.FrameNotes like '%FOC%' THEN 1 ELSE 0 END IsFOC,
			F.IsInsert, IsMultivision
		from dbo.PatientOrder po left join dbo.PatientOrderStatus pos
		on po.OrderNumber = pos.OrderNumber
		inner join Individual i on i.ID = po.IndividualID_Patient
		inner join Frame f on f.FrameCode = po.FrameCode
		where poS.DateLastModified between @FromDate and @ToDate and 
		pos.OrderStatusTypeID > 2 and pos.OrderStatusTypeID not in (3,4,5)
		and FrameNotes like ('JUSTIFY') AND 
		IsInsert = 0 and pos.IsActive = 1
	) a
)b
where LabSiteCode = @LabSiteCode
Group by LabSiteCode, FrameCode
order by LabSiteCode, FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetOverdueClinicOrders]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  4/2/15 - baf - To remove the standard 10 day over due calculation
--		and replace it with more accurate calculation based upon OFE CONOPS
--		and shipping maps for FedEx, UPS Ground and USPS -- Business Days.
--		1/4/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetOverdueClinicOrders]
	@FromDate DateTime,
	@ToDate DateTime,
	@SiteCode VarChar(6),
	@Priority VarChar(1) = null,
	@Status VarChar(2) = null,
	@ModifiedBy VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	------------ Audit Multi Read 10/23/14 - BAF
	Declare @ReadDate		DateTime	= GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'PatientIDs: ') + Cast(po.IndividualID_Patient as VarChar(20)) + ', '
	From	
	(
		select pos.OrderNumber, pos.DateLastModified as OrderDate,
			CASE WHEN po.IsMultivision = 0 and	SUBSTRING(po.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,2)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,5)	
				WHEN po.IsMultiVision = 0 and SUBSTRING(po.Demographic,8,1) = 'T' 
					then dbo.fn_AddBizDays(pos.DateLastModified,2)
				WHEN po.IsMultiVision = 1 and SUBSTRING(po.Demographic,8,1) = 'T' 
					Then dbo.fn_AddBizDays(pos.DateLastModified,5)	
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'P' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'P'
					then dbo.fn_AddBizDays(pos.DateLastModified,4)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)					
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'S' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,2)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'S'
					then dbo.fn_AddBizDays(pos.DateLastModified,5)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'F' 
					Then dbo.fn_AddBizDays(pos.DateLastModified,2)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'F'
					then dbo.fn_AddBizDays(pos.DateLastModified,5)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,4)
			END OrderDueDate--, 7 as MaxShipDays
		from dbo.PatientOrderStatus pos
			Inner Join dbo.PatientOrder po on pos.OrderNumber = po.OrderNumber
		where OrderStatusTypeID = 2
		and pos.IsActive = 1
	) a
	left join dbo.PatientOrder po on po.OrderNumber = a.OrderNumber
	inner join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	inner join dbo.IndividualIDNumber inIDNbr on inIDNbr.IndividualID = ind.ID
	inner join dbo.SiteCode scl on scl.SiteCode = po.LabSiteCode
	
	where DATEDIFF(d,a.OrderDueDate,GETDATE()) > 0
		and OrderDate between @FromDate and DATEADD(d,+1,@ToDate)  
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2))
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and inIDNbr.IsActive = 1
	Group by a.OrderNumber, po.LabSiteCode, po.ClinicSiteCode, po.FrameCode,
		po.Pairs, po.LensType, po.Tint, po.Coatings, a.OrderDate, a.OrderDueDate, --MaxShipDays,
		SiteName,ind.FirstName, ind.MiddleName, ind.LastName,IDNumber, 
		po.Demographic, po.IndividualID_Patient; 

	Select @Notes = Coalesce(@Notes, 'OverDue Order Numbers: ') + a.OrderNumber + ', '
	From
	(
		select pos.OrderNumber, pos.DateLastModified as OrderDate,
			CASE WHEN po.IsMultivision = 0 and	SUBSTRING(po.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,2)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,5)	
				WHEN po.IsMultiVision = 0 and SUBSTRING(po.Demographic,8,1) = 'T' 
					then dbo.fn_AddBizDays(pos.DateLastModified,2)
				WHEN po.IsMultiVision = 1 and SUBSTRING(po.Demographic,8,1) = 'T' 
					Then dbo.fn_AddBizDays(pos.DateLastModified,5)	
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'P' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'P'
					then dbo.fn_AddBizDays(pos.DateLastModified,4)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)					
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'S' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,2)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'S'
					then dbo.fn_AddBizDays(pos.DateLastModified,5)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'F' 
					Then dbo.fn_AddBizDays(pos.DateLastModified,2)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'F'
					then dbo.fn_AddBizDays(pos.DateLastModified,5)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,4)
			END OrderDueDate--, 7 as MaxShipDays
		--DATEADD(D,10,DateLastModified) as OrderDueDate, 
		--DATEDIFF(d,DateAdd(d,10,DateLastModified), GETDATE()) as DaysOverdue
		from dbo.PatientOrderStatus pos
			Inner Join dbo.PatientOrder po on pos.OrderNumber = po.OrderNumber
		where OrderStatusTypeID = 2
		and pos.IsActive = 1
	) a
	left join dbo.PatientOrder po on po.OrderNumber = a.OrderNumber
	inner join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	inner join dbo.IndividualIDNumber inIDNbr on inIDNbr.IndividualID = ind.ID
	inner join dbo.SiteCode scl on scl.SiteCode = po.LabSiteCode
	where DATEDIFF(d,a.OrderDueDate,GETDATE()) > 0 --dbo.fn_AddBizDays(a.OrderDueDate,MaxShipDays) > 0
		and OrderDate between @FromDate and DATEADD(d,+1,@ToDate)  
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2))
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and inIDNbr.IsActive = 1
	Group by a.OrderNumber, po.LabSiteCode, po.ClinicSiteCode, po.FrameCode,
		po.Pairs, po.LensType, po.Tint, po.Coatings, a.OrderDate, a.OrderDueDate, --a.MaxShipDays,
		SiteName,ind.FirstName, ind.MiddleName, ind.LastName,IDNumber, 
		po.Demographic
		
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	---------------------------------------------
    
	SELECT distinct a.OrderNumber, po.LabSiteCode, po.ClinicSiteCode, po.FrameCode,
		po.Pairs,po.LensType, po.Tint, po.Coatings, a.OrderDate, OrderDueDate,--MaxShipDays,
		--dbo.fn_AddBizDays(a.OrderDueDate,MaxShipDays) as OrderDueDate, 
		--DATEDIFF(d,a.OrderDueDate,GETDATE()) AS DaysOverDue,
		dbo.fnWrkDays(a.OrderDueDate, GETDATE()) as DaysOverDue,
		SUBSTRING(po.Demographic, 7, 1) AS sexcode,
		SUBSTRING(po.Demographic, 1, 3) AS MilRank,
		SUBSTRING(po.Demographic, 5,2) AS StatusCode,
		SUBSTRING(po.Demographic, 8, 1) AS OrderPriority,
		SUBSTRING(po.Demographic, 4, 1) AS bos,
		ind.FirstName, SUBSTRING (ind.MiddleName,1,1) as MI, 
		ind.LastName,scl.SiteName,  
		CASE WHEN LEN(DinIDNbr.IDNumber) = 0 THEN RIGHT(inIDNbr.IDNumber,4) 
			ELSE RIGHT(DinIDNbr.IDNumber,4) END IDNbr
	from
	(
		select pos.OrderNumber, pos.DateLastModified as OrderDate,
			CASE WHEN po.IsMultivision = 0 and	SUBSTRING(po.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,2)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'R' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,5)	
				WHEN po.IsMultiVision = 0 and SUBSTRING(po.Demographic,8,1) = 'T' 
					then dbo.fn_AddBizDays(pos.DateLastModified,2)
				WHEN po.IsMultiVision = 1 and SUBSTRING(po.Demographic,8,1) = 'T' 
					Then dbo.fn_AddBizDays(pos.DateLastModified,5)	
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'P' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'P'
					then dbo.fn_AddBizDays(pos.DateLastModified,4)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'V' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)					
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'S' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,2)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'S'
					then dbo.fn_AddBizDays(pos.DateLastModified,5)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'F' 
					Then dbo.fn_AddBizDays(pos.DateLastModified,2)
				When po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'F'
					then dbo.fn_AddBizDays(pos.DateLastModified,5)
				WHEN po.IsMultivision = 0 and SUBSTRING(po.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,1)
				WHEN po.IsMultivision = 1 and SUBSTRING(po.Demographic,8,1) = 'W' 
					THEN dbo.fn_AddBizDays(pos.DateLastModified,4)
			END OrderDueDate--, 7 as MaxShipDays
		--DATEADD(D,10,DateLastModified) as OrderDueDate, 
		--DATEDIFF(d,DateAdd(d,10,DateLastModified), GETDATE()) as DaysOverdue
		from dbo.PatientOrderStatus pos
			Inner Join dbo.PatientOrder po on pos.OrderNumber = po.OrderNumber
		where OrderStatusTypeID = 2
		and pos.IsActive = 1
	) a
	left join dbo.PatientOrder po on po.OrderNumber = a.OrderNumber
	inner join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	inner join dbo.IndividualIDNumber inIDNbr on inIDNbr.IndividualID = ind.ID
	inner join dbo.IndividualIDNumber DinIDNbr on DinIDNbr.IndividualID = ind.ID
	inner join dbo.SiteCode scl on scl.SiteCode = po.LabSiteCode
	where DATEDIFF(d,a.OrderDueDate,GETDATE()) > 0
		and OrderDate between @FromDate and DATEADD(d,+1,@ToDate)  
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2))
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and inIDNbr.IsActive = 1 AND inIDNbr.IDNumberType = 'SSN'
		AND DinIDNbr.IDNumberType = 'DIN'
	Group by a.OrderNumber, po.LabSiteCode, po.ClinicSiteCode, po.FrameCode,
		po.Pairs, po.LensType, po.Tint, po.Coatings, a.OrderDate, a.OrderDueDate, DATEDIFF(d,a.OrderDueDate,GETDATE()),
		SiteName,ind.FirstName, ind.MiddleName, ind.LastName,inIDNbr.IDNumber, DinIDNbr.IDNumber, 
		po.Demographic--, MaxShipDays
	Order by LabSiteCode, LastName, a.OrderNumber--, MaxShipDays
		
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetOtherFrames]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 10/9/2013
-- Description:	Other Frames (GITMO/KEFLAVIK, CG Dependents, 
--		Mission Only, NOSTRA - Oversize)
-- =============================================
CREATE PROCEDURE [dbo].[GetOtherFrames] 
	-- Add the parameters for the stored procedure here
	@FromDate Datetime,
	@ToDate Datetime,
	@LabSiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select LabSiteCode, FrameCode, SUM(NAD) as TNAD, SUM(NRet) as TNRET, 
	SUM(MAD) as TMAD, SUM(MRet) as TMRet, SUM(AAD) as TAAD, SUM(ARet) as TARet,
	SUM(FAD) as TFAD, sum(FRet) as TFRet, SUM(PDRL) as TPDRL, SUM(FmrPOW) as TPOW,
	SUM(CAD) as TCAD, SUM(PAD) as TPAD, SUM(BAD) as TBAD, SUM(FRes) as TFRES, 
	SUM(ARes) as TARes, SUM(ANG) as TANG, sum(FNG) as TFNG,SUM(NRes) as TNRes,
	SUM(NDepAD) as TNDep, SUM(MRes) as TMRes, SUM(DodRemCONUS) as TDRCONUS, 
	SUM(VABen) as TVABen, SUM(IMET) as TIMET, SUM(Other) as TOther
from
(
	select LabSiteCode, FrameCode,
		case when a.bos = 'N' and statuscode in (11,14) then Pairs else 0 end NAD,
		case when a.bos = 'N' and statuscode = 31 then Pairs else 0 end NRet,
		case when a.bos = 'M' and statuscode = 11 then Pairs else 0 end MAD,
		case when a.bos = 'M' and statuscode = 31 then Pairs else 0 end MRet,
		case when a.bos = 'A' and statuscode in (11,14) then Pairs else 0 end AAD,
		case when a.bos = 'A' and statuscode = 31 then Pairs else 0 end ARet,
		case when a.bos = 'F' and statuscode in (11,14) then Pairs else 0 end FAD,
		case when a.bos = 'F' and statuscode = 31 then Pairs else 0 end FRet,
		case when statuscode = 32 then Pairs else 0 end PDRL,
		case when statuscode = 36 then Pairs else 0 end FmrPOW,
		case when a.bos = 'C' and statuscode = 11 then Pairs else 0 end CAD,
		case when a.bos = 'P' and statuscode = 11 then Pairs else 0 end PAD,
		case when a.bos = 'B' and statuscode = 11 then Pairs else 0 end BAD,
		case when a.bos = 'F' and statuscode = 12 then Pairs else 0 end FRes,
		case when a.bos = 'A' and statuscode = 12 then Pairs else 0 end ARes,
		case when a.bos = 'A' and statuscode = 15 then Pairs else 0 end ANG,
		case when a.bos = 'F' and statuscode = 15 then Pairs else 0 end FNG,
		case when a.bos = 'N' and statuscode = 12 then Pairs else 0 end NRes,
		case when a.bos = 'N' and statuscode = 41 then Pairs else 0 end NDepAD,
		case when a.bos = 'M' and statuscode = 12 then Pairs else 0 end MRes,
		case when a.bos = 'K' and statuscode = 55 then Pairs else 0 end DodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 then Pairs else 0 end VABen,
		case when a.bos = 'K' and statuscode = 71 then Pairs else 0 end IMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) then Pairs else 0 end Other
	from
	(
		select pos.OrderStatusTypeID, po.ClinicSiteCode, po.LabSiteCode,
			REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(i.Demographic, 5,2) AS statuscode,
			SUBSTRING(i.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(i.Demographic, 4, 1) AS bos, po.FrameCode, po.LensType,
			po.Pairs, f.FrameDescription, f.FrameNotes,
			case when f.FrameNotes like '%FOC%' THEN 1 ELSE 0 END IsFOC,
			case when f.FrameNotes like '%OVERSIZE%' Or f.FrameNotes like '%GITMO/KEFLAVIK%' 
				OR f.FrameNotes like '%DEPENDENTS%' THEN 1 ELSE 0 END IsOther,
			F.IsInsert, IsMultivision
		from dbo.PatientOrder po left join dbo.PatientOrderStatus pos
		on po.OrderNumber = pos.OrderNumber
		inner join Individual i on i.ID = po.IndividualID_Patient
		inner join Frame f on f.FrameCode = po.FrameCode
		where poS.DateLastModified between @FromDate and @ToDate and 
		pos.OrderStatusTypeID > 2 and pos.OrderStatusTypeID not in (3,4,5)
		and f.FrameNotes like '%OVERSIZE%' Or f.FrameNotes like '%GITMO/KEFLAVIK%' 
				OR f.FrameNotes like '%DEPENDENTS%' and IsInsert = 0 
		and pos.IsActive = 1
	) a
	where IsOther = 1
)b
where LabSiteCode = @LabSiteCode
Group by LabSiteCode, FrameCode
order by LabSiteCode, FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrderStateHistoryByOrderNumber]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 5/24/2015
-- Description:	This stored procedure returns the order status history
--		for a specified order number, latest status first.
-- Modified:  7/8/2015 - baf - added OrderStatusDescription
-- =============================================
CREATE PROCEDURE [dbo].[GetOrderStateHistoryByOrderNumber]
	
	@OrderNumber VarChar(16) 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Select pos.*, OrderStatusDescription from dbo.PatientOrderStatus pos
	left join dbo.OrderStatusType ost on pos.OrderStatusTypeID = ost.ID
	Where OrderNumber = @OrderNumber
	order by pos.DateLastModified Desc
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrdersForTransferToLabByLabCodeTest]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified: 1/4/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetOrdersForTransferToLabByLabCodeTest] 
@SiteCode	VARCHAR(6),
@ModifiedBy	VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	---------- Auditing Multi Read 10/21/14 - BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'Patients: ') + Cast(a.IndividualID_Patient as VarChar(20)) + ', '  
	FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.ID
		INNER JOIN dbo.PatientOrderStatus e ON a.OrderNumber = e.OrderNumber
		LEFT OUTER JOIN dbo.IndividualPhoneNumber i ON a.IndividualID_Patient = i.IndividualID
		LEFT OUTER JOIN dbo.IndividualEMailAddress j ON a.IndividualID_Patient = j.IndividualID
		WHERE e.LabSiteCode = @SiteCode
		AND e.OrderStatusTypeID = 1
		AND e.IsActive = 1
		AND a.IsGEyes = 0
		AND a.OnholdForConfirmation = 0
		
	Select @Notes = Coalesce(@Notes, 'OrderNumbers: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.ID
		INNER JOIN dbo.PatientOrderStatus e ON a.OrderNumber = e.OrderNumber
		LEFT OUTER JOIN dbo.IndividualPhoneNumber i ON a.IndividualID_Patient = i.IndividualID
		LEFT OUTER JOIN dbo.IndividualEMailAddress j ON a.IndividualID_Patient = j.IndividualID
		WHERE e.LabSiteCode = @SiteCode
		AND e.OrderStatusTypeID = 1
		AND e.IsActive = 1
		AND a.IsGEyes = 0
		AND a.OnholdForConfirmation = 0
		
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	--------------------------------------------

	SELECT
	1 AS tag,
	NULL AS parent,
		a.OrderNumber AS [order!1!ordernumber!element],
		SUBSTRING(a.Demographic, 7, 1) AS [order!1!sexcode!element],
		REPLACE(SUBSTRING(a.Demographic, 1, 3), '*', '') AS [order!1!rankcode!element],
		SUBSTRING(a.Demographic, 5,2) AS [order!1!statuscode!element],
		SUBSTRING(a.Demographic, 8, 1) AS [order!1!orderpriority!element],
		SUBSTRING(a.Demographic, 4, 1) AS [order!1!bos!element],
		SUBSTRING(f.FirstName, 1,1) + SUBSTRING(f.MiddleName, 1,1) + SUBSTRING(f.LastName,1,1) AS [order!1!technicianinitials!element],
		a.LensType AS [order!1!lenstype!element],
		a.LensMaterial AS [order!1!lensmaterial!element],
		a.Tint AS [order!1!tint!element],
		a.Coatings AS [order!1!coatings!element], -- added 1/4/18
		a.ODSegHeight AS [order!1!odsegheight!element],
		a.OSSegHeight AS [order!1!ossegheight!element],
		a.NumberOfCases AS [order!1!cases!element],
		a.Pairs AS [order!1!pairs!element],
		a.FrameCode AS [order!1!framecode!element],
		a.FrameColor AS [order!1!framecolor!element],
		a.FrameEyeSize AS [order!1!frameeyesize!element],
		a.FrameBridgeSize AS [order!1!framebridgesize!element],
		a.FrameTempleType AS [order!1!frametempletype!element],
		b.ODPDDistant AS [order!1!odpddistant!element],
		b.OSPDDistant AS [order!1!ospddistant!element],
		b.PDDistant AS [order!1!pddistant!element],
		b.ODPDNear AS [order!1!odpdnear!element],
		b.OSPDNear AS [order!1!ospdnear!element],
		b.PDNear AS [order!1!pdnear!element],
		a.ClinicSiteCode AS [order!1!clinicsitecode!element],
		a.LabSiteCode AS [order!1!labsitecode!element],
		a.ShipToPatient AS [order!1!shiptopatient!element],
		a.ShipAddress1 AS [order!1!shipaddress1!element],
		a.ShipAddress2 AS [order!1!shipaddress2!element],
		a.ShipCity AS [order!1!shipcity!element],
		a.ShipState AS [order!1!shipstate!element],
		a.ShipZipCode AS [order!1!shipzipcode!element],
		a.ShipAddressType AS [order!1!shipaddresstype!element],
		a.LocationCode AS [order!1!theaterlocationcode!element],
		a.UserComment1 AS [order!1!usercomment1!element],
		a.UserComment2 AS [order!1!usercomment2!element],
        a.IsGEyes AS [order!1!isgeyes!element],
        a.IsMultivision AS [order!1!ismultivision!element],
        a.VerifiedBy AS [order!1!verifiedby!element],
        b.ODAdd AS [order!1!odadd!element],
        b.ODAxis AS [order!1!odaxis!element],
        b.ODCylinder AS [order!1!odcylinder!element],
        b.ODHBase AS [order!1!odhbase!element],
        b.ODHPrism AS [order!1!odhprism!element],
        b.ODSphere AS [order!1!odsphere!element],
        b.ODVBase AS [order!1!odvbase!element],
        b.ODVPrism AS [order!1!odvprism!element],
        b.OSAdd AS [order!1!osadd!element],
        b.OSAxis AS [order!1!osaxis!element],
        b.OSCylinder AS [order!1!oscylinder!element],
        b.OSHBase AS [order!1!oshbase!element],
        b.OSHPrism AS [order!1!oshprism!element],
        b.OSSphere AS [order!1!ossphere!element],
        b.OSVBase AS [order!1!osvbase!element],
        b.OSVPrism AS [order!1!osvprism!element],
        c.ODCorrectedAcuity AS [order!1!odcorrectedacuity!element],
        c.ODOSCorrectedAcuity AS [order!1!odoscorrectedacuity!element],
        c.ODOSUncorrectedAcuity AS [order!1!odosuncorrectedacuity!element],
        c.ODUncorrectedAcuity AS [order!1!oduncorrectedacuity!element],
        c.OSCorrectedAcuity AS [order!1!oscorrectedacuity!element],
        c.OSUncorrectedAcuity AS [order!1!osuncorrectedacuity!element],
        d.LastName + ', ' + d.FirstName + ' ' + SUBSTRING(d.MiddleName, 1, 1) + '.' AS [order!1!name!element],
        g.LastName + ', ' + g.FirstName AS [order!1!doctor!element],
        c.ExamDate AS [order!1!examdate!element],
        h.IDNumber AS [order!1!idnumber!element],
        '(' + i.AreaCode + ')-' + i.PhoneNumber AS [order!1!phonenumber!element],
        j.EMailAddress AS [order!1!emailaddress!element],
        e.DateLastModified as [order!1!orderdate!element] 
		FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.ID
		INNER JOIN dbo.PatientOrderStatus e ON a.OrderNumber = e.OrderNumber
		LEFT OUTER JOIN dbo.IndividualPhoneNumber i ON a.IndividualID_Patient = i.IndividualID
		LEFT OUTER JOIN dbo.IndividualEMailAddress j ON a.IndividualID_Patient = j.IndividualID
		WHERE e.LabSiteCode = @SiteCode
		AND e.OrderStatusTypeID = 1
		AND e.IsActive = 1
		AND a.IsGEyes = 0
		AND a.OnholdForConfirmation = 0
		ORDER BY a.OrderNumber
		FOR XML EXPLICIT, ROOT('orders')
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrdersForTransferToLabByLabCode]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 30 Jul 14
-- Description:	Gets a list of orders ready for transfer to a lab by site code.
--				It is the same as GetOrdersForTransferToLabByLabCodeTest except it's
--				result set is not in an XML format.
-- Modified: 1/4/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetOrdersForTransferToLabByLabCode]
	@siteCode		varchar(6),
	@ModifiedBy		VarChar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	---------- Auditing MultiRead 10/21/14 - BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce (@PatientList, 'Patient List: ') + CAST(a.IndividualID_Patient as VarChar(20)) + ', '
	FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.ID
		INNER JOIN dbo.PatientOrderStatus e ON a.OrderNumber = e.OrderNumber
		LEFT OUTER JOIN dbo.IndividualPhoneNumber i ON a.IndividualID_Patient = i.IndividualID
		LEFT OUTER JOIN dbo.IndividualEMailAddress j ON a.IndividualID_Patient = j.IndividualID
	WHERE e.LabSiteCode = @siteCode
		AND e.OrderStatusTypeID = 1
		AND e.IsActive = 1
		And J.IsActive = 1
		and i.IsActive = 1
		AND a.IsGEyes = 0
		AND a.OnholdForConfirmation = 0 
		
	Select @Notes = Coalesce(@Notes, 'OrderNumbers: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.ID
		INNER JOIN dbo.PatientOrderStatus e ON a.OrderNumber = e.OrderNumber
		LEFT OUTER JOIN dbo.IndividualPhoneNumber i ON a.IndividualID_Patient = i.IndividualID
		LEFT OUTER JOIN dbo.IndividualEMailAddress j ON a.IndividualID_Patient = j.IndividualID
	WHERE e.LabSiteCode = @siteCode
		AND e.OrderStatusTypeID = 1
		AND e.IsActive = 1
		And J.IsActive = 1
		and i.IsActive = 1
		AND a.IsGEyes = 0
		AND a.OnholdForConfirmation = 0 
		
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12,@PatientList,@Notes	
	
	-------------------------------------
	
	SELECT
		a.OrderNumber,
		SUBSTRING(a.Demographic, 7, 1) as SexCode,
		REPLACE(SUBSTRING(a.Demographic, 1, 3), '*', '') as RankCode,
		SUBSTRING(a.Demographic, 5,2) as StatusCode,
		SUBSTRING(a.Demographic, 8, 1) as OrderPriority,
		SUBSTRING(a.Demographic, 4, 1) as BOS,
		SUBSTRING(f.FirstName, 1,1) + SUBSTRING(f.MiddleName, 1,1) + SUBSTRING(f.LastName,1,1) as TechnicianInitials,
		a.LensType,
		a.LensMaterial,
		a.Tint,
		a.Coatings, -- added 1/4/18
		a.ODSegHeight,
		a.OSSegHeight,
		a.NumberOfCases,
		a.Pairs,
		a.FrameCode,
		a.FrameColor,
		a.FrameEyeSize,
		a.FrameBridgeSize,
		a.FrameTempleType,
		b.ODPDDistant,
		b.OSPDDistant,
		b.PDDistant,
		b.ODPDNear,
		b.OSPDNear,
		b.PDNear,
		a.ClinicSiteCode,
		a.LabSiteCode,
		a.ShipToPatient,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipCity,
		a.ShipState,
		a.ShipZipCode,
		a.ShipAddressType,
		a.LocationCode,
		a.UserComment1,
		a.UserComment2,
		a.IsGEyes,
		a.IsMultivision,
		a.VerifiedBy,
		b.ODAdd,
		b.ODAxis,
		b.ODCylinder,
		b.ODHBase,
		b.ODHPrism,
		b.ODSphere,
		b.ODVBase,
		b.ODVPrism,
		b.OSAdd,
		b.OSAxis,
		b.OSCylinder,
		b.OSHBase,
		b.OSHPrism,
		b.OSSphere,
		b.OSVBase,
		b.OSVPrism,
		c.ODCorrectedAcuity,
		c.ODOSCorrectedAcuity,
		c.ODOSUncorrectedAcuity,
		c.ODUncorrectedAcuity,
		c.OSCorrectedAcuity,
		c.OSUncorrectedAcuity,
		d.LastName + ', ' + d.FirstName + ' ' + SUBSTRING(d.MiddleName, 1, 1) + '.' as Name,
		g.LastName + ', ' + g.FirstName as Doctor,
		c.ExamDate,
		h.IDNumber,
		i.AreaCode + i.PhoneNumber as PhoneNumber,
		--'(' + i.AreaCode + ')-' + i.PhoneNumber as PhoneNumber,
		j.EMailAddress,
		e.DateLastModified
	FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.ID
		INNER JOIN dbo.PatientOrderStatus e ON a.OrderNumber = e.OrderNumber
		LEFT OUTER JOIN dbo.IndividualPhoneNumber i ON a.IndividualID_Patient = i.IndividualID
		LEFT OUTER JOIN dbo.IndividualEMailAddress j ON a.IndividualID_Patient = j.IndividualID
	WHERE e.LabSiteCode = @siteCode
		AND e.OrderStatusTypeID = 1
		AND e.IsActive = 1
		And J.IsActive = 1
		and i.IsActive = 1
		AND a.IsGEyes = 0
		AND a.OnholdForConfirmation = 0
	ORDER BY a.OrderNumber
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrdersByUnitRpt]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified: 1/5/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetOrdersByUnitRpt]
	-- Add the parameters for the stored procedure here
	@FromDate DateTime,
	@ToDate DateTime,
	@SiteCode VarChar(6) = null,
	@UIC VarChar(6) = null,
	@OrderType VarChar(15),
	@Priority VarChar(1) = null,
	@Status VarChar(2) = NULL,
	@ModifiedBy	VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	------------------ Audit MultiRead - 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientLIst = COALESCE(@PatientList, 'Patient IDs: ') + CAST(po.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder po Left Join  dbo.IndividualIDNumber id
			on po.IndividualID_Patient = id.IndividualID
		left join Individual ind on id.IndividualID = ind.ID
		inner join dbo.IndividualAddress ia on ia.IndividualID = ind.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	Where pos.DateLastModified between @FromDate and @ToDate
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1)= ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and pos.OrderStatusTypeID in (1,6) -- GEyes or Clinic Ordered
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		and (@UIC is null or ia.UIC = @UIC)
		and id.IsActive = 1
		and pos.IsActive = 1
		
	SELECT @Notes = COALESCE(@Notes, 'Unit Orders: ') + po.OrderNumber + ', '
	FROM dbo.PatientOrder po Left Join  dbo.IndividualIDNumber id
			on po.IndividualID_Patient = id.IndividualID
		left join Individual ind on id.IndividualID = ind.ID
		inner join dbo.IndividualAddress ia on ia.IndividualID = ind.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	Where pos.DateLastModified between @FromDate and @ToDate
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1)= ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and pos.OrderStatusTypeID in (1,6) -- GEyes or Clinic Ordered
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		and (@UIC is null or ia.UIC = @UIC)
		and id.IsActive = 1
		and pos.IsActive = 1 
		
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	---------------------------------------------------

IF (@OrderType = 'Ordered')
-- Get Orders with Order Date Only
	BEGIN
	SELECT distinct po.ClinicSiteCode, po.LabSiteCode, ind.LastName, ind.FirstName, 
		SUBSTRING(ind.MiddleName,1,1) as MI, RIGHT(id.IDNumber,4) as LastFour,
		po.OrderNumber, po.FrameCode, Pairs, LensType, Tint, Coatings, -- added 1/5/18
		SUBSTRING(po.demographic,5,2) as StatusCode,
		SUBSTRING(po.demographic,8,1) as PriorityCode,
		SUBSTRING(po.demographic,4,1) as BOS,
		REPLACE(SUBSTRING(po.demographic,1,3),'*','') as MilRank,
		ia.UIC, Null as RecDte,
		pos.DateLastModified as OrdDte,
		'0' as OrdDays
	FROM dbo.PatientOrder po Left Join  dbo.IndividualIDNumber id
			on po.IndividualID_Patient = id.IndividualID
		left join Individual ind on id.IndividualID = ind.ID
		inner join dbo.IndividualAddress ia on ia.IndividualID = ind.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	Where pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1)= ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and pos.OrderStatusTypeID in (1,6) -- GEyes or Clinic Ordered
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		and (@UIC is null or ia.UIC = @UIC)
		and id.IsActive = 1
		and pos.IsActive = 1
	END
Else
-- Get Orders with Both Order and Dispense Dates
	BEGIN
		SELECT distinct po.ClinicSiteCode, po.LabSiteCode, ind.LastName, ind.FirstName, 
			SUBSTRING(ind.MiddleName,1,1) as MI, RIGHT(id.IDNumber,4) as LastFour,
			po.OrderNumber, po.FrameCode, Pairs, LensType, Tint, Coatings, -- Added 1/5/18
			SUBSTRING(po.demographic,5,2) as StatusCode,
			SUBSTRING(po.demographic,8,1) as PriorityCode,
			SUBSTRING(po.demographic,4,1) as BOS,
			REPLACE(SUBSTRING(po.demographic,1,3),'*','') as MilRank,
			ia.UIC, posr.DateLastModified as RecDte,
			pos.DateLastModified as OrdDte,
			case when posr.DateLastModified IS Not Null then	
				DATEDIFF(d,pos.DateLastModified,posr.DateLastModified) else '0' end OrdDays
		FROM dbo.PatientOrder po Left Join  dbo.IndividualIDNumber id
				on po.IndividualID_Patient = id.IndividualID
			left join Individual ind on id.IndividualID = ind.ID
			inner join dbo.IndividualAddress ia on ia.IndividualID = ind.ID
			inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
			inner join dbo.PatientOrderStatus posr on po.OrderNumber = posr.OrderNumber
		Where pos.DateLastModified between @FromDate and @ToDate
			and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
			and SUBSTRING(po.Demographic,8,1)= ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
			and pos.OrderStatusTypeID in (1,6) -- GEyes or Clinic Ordered
			and posr.OrderStatusTypeID in (7,8, 19) -- Clinic or Lab Dispensed
			and pos.DateLastModified < posr.DateLastModified
			and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
			and (@UIC is null or ia.UIC = @UIC)
			and id.IsActive = 1
	End	
END
GO
/****** Object:  StoredProcedure [dbo].[GetOrderNumbersForCheckIn]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetOrderNumbersForCheckIn]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 	SELECT a.LabSiteCode, a.OrderNumber 
    FROM 
	(SELECT ROW_NUMBER() OVER(PARTITION BY OrderNumber ORDER BY DateLastModified DESC) 
	AS Seq,* FROM dbo.PatientOrderStatus) t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber
	WHERE Seq = 1 
	AND (t.OrderStatusTypeID = 1 
	OR t.OrderStatusTypeID = 9 
	OR t.OrderStatusTypeID = 4
	OR t.OrderStatusTypeID = 10
	OR t.OrderStatusTypeID = 6)
	AND a.IsActive = 1 
	AND a.IsGEyes = 0
	AND a.LabSiteCode = @SiteCode
	GROUP BY a.LabSiteCode, a.OrderNumber
END
GO
/****** Object:  StoredProcedure [dbo].[GetLMSFiles]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		Kevin Bush
 Create date: 9 Apr 14
 Description:	This is a similar sp to Get711DataByLabCode but will have a few more fields.
				This sp was replicated due to potential issues with SSRS and the regeneration of reports
				when changes are made to the sp.
				This sp gets all the necessary data to build a pipe delimited file to ship off to NOSTRA.
 Modified:  KDB - 3/12/15 - PD Changes
			JRP - 4/28/15 - Removed extraneous function calls		
 	BAF - 5/15/2015 -- Used NOSTRA sp for an admin sp to view LMS Records.  Removed record status
		check, so that all orders can be viewed as sent to the LMS Systems.
	BAF - 5/27/16 - Added TBI Tints
    BAF - 8/1/16 - Modified Codes for Pink Specialty Tints
	BAF = 2/23/17 - Added Dispense Comments
	BAF - 1/10/18 - Added Coatings
 =============================================*/
CREATE PROCEDURE [dbo].[GetLMSFiles]
@SiteCode	VARCHAR(6) = Null,
@OrderNbr Varchar(16) = Null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @orderPriority varchar(1);
	--set @orderPriority = select upper(SUBSTRING(p.Demographic, 8, 1)) 
	--from PatientOrder p 
	--where (p.ClinicSiteCode is null or p.ClinicSiteCode = @SiteCode) 
	--and (p.OrderNumber is null or p.OrderNumber = @OrderNbr);
	
	------ Audit MultiRead Routine -- 10/16/14 - BAF
	---  Multiple Records for one patient
	Declare @ReadDate		DateTime = GetDate()
	Declare @ModifiedBy		VarChar(200) = 'WebService'
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)

	Select @Notes = Coalesce (@Notes, 'NOSTRA File Data: ') + a.OrderNumber + ', '
	 FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    LEFT OUTER JOIN dbo.IndividualAddress ia ON a.IndividualID_Patient = ia.ID and ia.UIC = 'UNIT'
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.LookupTable bc on SUBSTRING(a.Demographic, 4, 1) + SUBSTRING(a.Demographic, 5, 2) = bc.[Text]
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	WHERE a.IsActive = 1 and a.ModifiedBy <> 'SSIS'
	AND b.IsActive = 1
	AND k.IsActive = 1
	AND b.OrderStatusTypeID in (1,6,4,9,10)
	--AND (b.OrderStatusTypeID = 1 OR b.OrderStatusTypeID = 6 OR b.OrderStatusTypeID = 4 
	--OR b.OrderStatusTypeID = 9 OR b.OrderStatusTypeID = 10)
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND bc.Code = 'VStarBillCode'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	AND (@SiteCode is Null or a.LabSiteCode = @SiteCode)
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
	
	Select @PatientList = Coalesce(@PatientList, 'Patients: ') + CAST(a.IndividualID_Patient AS VARCHAR) + ', '
	FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    LEFT OUTER JOIN dbo.IndividualAddress ia ON a.IndividualID_Patient = ia.ID and ia.UIC = 'UNIT'
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.LookupTable bc on SUBSTRING(a.Demographic, 4, 1) + SUBSTRING(a.Demographic, 5, 2) = bc.[Text]
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	WHERE a.IsActive = 1 and a.ModifiedBy <> 'SSIS'
	AND b.IsActive = 1
	AND k.IsActive = 1
	AND b.OrderStatusTypeID in (1,6,4,9,10)
	--AND (b.OrderStatusTypeID = 1 OR b.OrderStatusTypeID = 6 OR b.OrderStatusTypeID = 4 
	--OR b.OrderStatusTypeID = 9 OR b.OrderStatusTypeID = 10)
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND bc.Code = 'VStarBillCode'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	AND (@SiteCode is Null or a.LabSiteCode = @SiteCode)
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
	
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 22, @PatientList, @Notes
	----------------------------
	
	SELECT distinct
		a.OrderNumber,
		Left(a.OrderNumber,13) as DocumentNumber,
		--a.ClinicSiteCode + convert(varchar(9), a.OrderNumber) as DocumentNumber,
		a.Demographic,
		SUBSTRING(a.Demographic, 7, 1) AS sexcode,
		SUBSTRING(a.Demographic, 1, 3) AS rankcode,
		SUBSTRING(a.Demographic, 5,2) AS statuscode,
		--SUBSTRING(a.Demographic, 8, 1) AS orderpriority,
		case 
			when upper(RIGHT(a.Demographic, 1)) = 'F' then 'FOC'
			when upper(RIGHT(a.Demographic, 1)) = 'P' then 'DWN PILOT'
			when upper(RIGHT(a.Demographic, 1)) = 'R' then 'READINESS'
			when upper(RIGHT(a.Demographic, 1)) = 'T' then 'TRAINEE'
			when upper(RIGHT(a.Demographic, 1)) = 'V' then 'VIP'
			when upper(RIGHT(a.Demographic, 1)) = 'W' then 'W'
			when upper(RIGHT(a.Demographic, 1)) = 'S' then 'STANDARD'
			else ''
		end as orderpriority,
		SUBSTRING(a.Demographic, 4, 1) AS bos,
		bc.Value as BillingCode,
		SUBSTRING(m.FirstName, 1,1) + SUBSTRING(m.MiddleName, 1,1) + SUBSTRING(m.LastName,1,1) AS techinitials,
		
		-- If lenstype is and single vision or half eye lens, then 
		--case 
		--	when upper(a.LensType) = 'SVD' or UPPER(a.LensType) = 'SVN' or UPPER(a.LensType) = 'SVAL' or upper(a.LensType) = 'HLF' 
		--	then 
		--		case 
		--			when UPPER(a.LensType) = 'SVD'
		--				then 
		--					convert(varchar(6), PDDistant)
		--				else -- It is a near lens so use the near measurement with a M at the end
		--					convert(varchar(6), PDNear)
		--		end
		--	else -- It is a multivision lens and uses both measurements and the distant measurement has a B after it
		--		convert(varchar(6), a.PDDistant) + 'B ' + convert(varchar(6), a.PDNear)
		--end as PupillaryDistant,
		
		case 
			when a.LensMaterial = 'PLAS' then 'CPLS'
			when a.LensMaterial = 'POLY' then 'CPLY'
			when a.LensMaterial = 'HI' then 'HPLS'
			when a.LensMaterial = 'TRAN' then 'TRNG'
			when a.LensMaterial = 'LSR' then 'LSR'
			when a.LensMaterial = 'GLAS' then 'GLAS'
			else ''
		end as LensMaterial,
		
		'COMP' as LensStatus,
		--a.Tint,
		
		case
			when a.LensType = 'BI' then '28'
			when a.LensType = 'BIW' then '35'
			when a.LensType = 'TRI' then '7X28'
			when a.LensType = 'TRIW' then '8X35'
			when a.LensType = 'BIAL' then 'STAL'
			when a.LensType = 'SVD' then 'SV'
			when a.LensType = 'SVN' then 'SV'
			when a.LensType = 'QUAD' then 'QUAD'
			when a.LensType = 'DD' then 'DD'
			when a.LensType = 'SVAL' then 'SVAL'
			else ''
		end as LensType,
		a.LensType as RawLensType,
		case 
			when a.ODSegHeight = '3' then '3D'
			when a.ODSegHeight = '4' then '4D'  
			else a.ODSegHeight end as ODSegHeight,
		case 
			when a.OSSegHeight = '3' then '3D' 
			when a.OSSegHeight = '4' then '4D'
			else a.OSSegHeight end as OSSegHeight,
		a.NumberOfCases,
		a.Pairs,
		CASE WHEN UPPER(a.FrameCode) = 'HLF' THEN 'HALF EYE FRAME' ELSE '' END AS LmsComment,
		a.FrameCode,
		a.FrameColor,
		a.FrameEyeSize,
		a.FrameBridgeSize,
		case when upper(right(a.FrameTempleType, 1)) = 'C' then a.FrameTempleType + 'Z' else a.FrameTempleType end as FrameTempleType,
		
		g.ODPDDistant,
		g.OSPDDistant,
		g.PDDistant,
		g.ODPDNear,
		g.OSPDNear,
		g.PDNear,
		
		/*
		a.ODPDDistant,
		a.OSPDDistant,
		a.PDDistant,
		a.ODPDNear,
		a.OSPDNear,
		a.PDNear,
		*/
		
		a.ClinicSiteCode,
		c.SiteName AS clinicname,
		e.Address1 AS clinicaddress1,
		e.Address2 AS clinicaddress2,
		e.Address3 as clinicaddress3,
		e.City AS cliniccity,
		e.Country AS cliniccountry,
		e.State AS clinicstate,
		e.ZipCode AS cliniczipcode,
		b.LabSiteCode,
		d.SiteName AS labname,
		f.Address1 AS labaddress1,
		f.Address2 AS labaddress2,
		f.Address3 as labaddress3,
		f.City AS labcity,
		f.Country AS labcountry,
		f.State AS labstate,
		f.ZipCode AS labzipcode,
		case when a.ShipToPatient = 1 then 'Y' else 'N' end as ShipToPatient,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipAddress3,
		a.ShipCity,
		a.ShipState,
		case when LEN(a.ShipZipCode) > 5 then LEFT(a.ShipZipCode,5) end BaseZip,
		case when LEN(a.shipZipCode) > 5 then RIGHT(a.ShipZipCode,4) end ZipPlus,
		a.ShipZipCode,
		a.ShipAddressType,
		a.LocationCode,
		replace(a.UserComment1,'|','_') as UserComment1,
		replace(a.UserComment2,'|','_') as UserComment2,
		'' as UserComment3,
		'' as UserComment4,
		'' as UserComment5,
		'' as UserComment6,
        a.IsGEyes,
        a.IsMultivision,
        a.VerifiedBy,
        a.PatientEmail,
        a.DateLastModified AS dateordercreated,
        a.OSDistantDecenter as osdistantdecenter,
        a.ODDistantDecenter as oddistantdecenter,
        a.OSNearDecenter as osneardecenter,
        a.ODNearDecenter as odneardecenter,
        a.ODBase as odbase,
        a.OSBase as osbase,
        g.ODAdd,
        g.ODAxis,
        g.OSAdd,
        g.OSAxis,
        g.ODCylinder,
        --case when upper(g.ODCylinder) <> 'SPHERE' then g.ODCylinder end as ODCylinder,
		g.OSCylinder,
		--case when UPPER(g.OSCylinder) <> 'SHPERE' then g.OSCylinder end as OSCylinder,
        
        -- If Prism is blank then base is blank
        case when g.ODHPrism = 0.00 then '' else CONVERT(varchar(6), g.ODHPrism) end as ODHPrism, 
        case when g.ODHPrism = 0.00 then '' else g.ODHBase end as ODHBase,
        
        case when g.ODVPrism = 0.00 then '' else convert(varchar(6), g.ODVPrism) end as ODVPrism,
        case when g.ODVPrism = 0.00 then '' else g.ODVBase end as ODVBase,
        
        case when g.OSHPrism = 0.00 then '' else convert(varchar(6), g.OSHPrism) end as OSHPrism,
        case when g.OSHPrism = 0.00 then '' else g.OSHBase end as OSHBase,
        
        case when g.OSVPrism = 0.00 then '' else convert(varchar(6), g.OSVPrism) end as OSVPrism,
        case when g.OSVPrism = 0.00 then '' else g.OSVBase end as OSVBase,
        
        -- THIS APPLYS TO BOTH OD AND OS
        -- If ODHPrism is blank then add ODV prism and base
        -- Else add ODH prism and base, add a space, add ODV prism and base
        case when ODHPrism = 0.00
			then cast(g.ODVPrism as varchar) + g.ODVBase
			else cast(g.ODHPrism as varchar) + g.ODHBase
			+ ' ' 
			+ cast(g.ODVPrism as varchar) + g.ODVBase end as ODPrism,
		
		case when OSHPrism = 0.00
			then cast(g.OSVPrism as varchar) + g.OSVBase
			else cast(g.OSHPrism as varchar) + g.OSHBase + ' ' + cast(g.OSVPrism as varchar) + g.OSVBase end as OSPrism,
        
        g.ODSphere,
        g.OSSphere,
        
        h.ODCorrectedAcuity,
        h.ODOSCorrectedAcuity,
        h.ODOSUncorrectedAcuity,
        h.ODUncorrectedAcuity,
        h.OSCorrectedAcuity,
        h.OSUncorrectedAcuity,
        h.examdate,
        i.FirstName,
        i.MiddleName,
        i.LastName,
        j.LastName + ', ' + j.FirstName AS doctor,
        k.IDNumber AS patientidnumber,
        l.AreaCode + '  ' + l.PhoneNumber AS patientphonenumber,
        n.[Text] as BOSDesc,
        o.[text] as StatDesc,
        p.[Text] as OrdPriDesc,
   --     case
			--when q.[Value] = '15' then 'N-15 Drk Grey'
			--when q.[Value] = '31' then 'N-31 Med Grey'
			--when q.[Value] = '40' then 'Nom. 60% Trans'
			--when q.[Value] = 'AR' then 'Anti-Refl'
			--when q.[Value] = 'CL' then 'Clear'
			--when q.[Value] = '51' then 'S-1 Lt Pink'
			--when q.[Value] = '52' then 'S-2 Med Pink'
			--when q.[Value] = 'UV' then 'UV-400'
   --     end TintDesc,
   
   /*
		If ProcType = TNT
		  @tint = vStarCode
		else
		  @tint = srtsCode

		VStarCode	SrtsCode	ProcType
		AR			AR			ARC
		N15			15			TNT
		N31			31			TNT
		N60			40			TNT
		SL1			S1			TNT
		SL2			S2			TNT
		UV4			UV			UVC
		SL3			S3			TNT
   */
		case 
	--		when q.Value = 'AR' then 'AR'
			when q.Value = '15' then 'N15'
			when q.Value = '31' then 'N31'
			when q.Value = '40' then 'N60'
			when q.Value = 'S1' then 'SL1'
			when q.Value = 'S2' then 'SL2'
	--		when q.Value = 'UV' then 'UVA'
			when q.Value = 'S3' then 'SL3'
			--when q.Value = 'CL' then 'CLR'
			-- specialty tints for TBI
			when q.Value = '60' then 'N40' -- Gray Nominal 40% Transmission
			when q.Value = 'P8' then 'PK85' -- Pink 85%
			when q.Value = 'P6' then 'PK60' -- Pink 60%
			when q.Value = 'A8' then 'AM80' -- Amber 80%
			when q.Value = 'A6' then 'AM60' -- Amber 60%
			when q.Value = 'B8' then 'BL85' -- Blue 85%
			when q.Value = 'B6' then 'BL65' -- Blue 65%
			when q.Value = 'FL' then 'FL41' -- Rose 50%
			else ''
		end Tint,	
		--q.Text as TintDesc,
        r.[Text] as ColorDesc,
        s.[Text] as MaterialDesc,
        t.[text] as LensDesc,
        a.ONBarCode,
        k.IDNumber as IndividualID_Patient,
        ia.UIC as Unit,
        replace(a.DispenseComments,'|','_') as DispenseComments, -- Added 2/23/17
        CASE WHEN a.Coatings = 'AR' THEN 'AR' -- Added 1/10/18
			WHEN a.coatings = 'UV' THEN 'UV4'
			WHEN a.Coatings = 'AR/UV' THEN 'ARU'
			WHEN a.Coatings = 'UV/AR' THEN 'ARU'
			else ''
		END Coating
    FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    LEFT OUTER JOIN dbo.IndividualAddress ia ON a.IndividualID_Patient = ia.ID and a.IsActive = 1 --and ia.UIC = 'UNIT'  *** Modified 5/6/16 baf
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.LookupTable bc on SUBSTRING(a.Demographic, 4, 1) + SUBSTRING(a.Demographic, 5, 2) = bc.[Text]
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
WHERE a.IsActive = 1 and a.ModifiedBy <> 'SSIS'
	AND b.IsActive = 1
	AND k.IsActive = 1
	AND b.OrderStatusTypeID in (1,6,4,9,10)
	--AND (b.OrderStatusTypeID = 1 OR b.OrderStatusTypeID = 6 OR b.OrderStatusTypeID = 4 
	--OR b.OrderStatusTypeID = 9 OR b.OrderStatusTypeID = 10)
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND bc.Code = 'VStarBillCode'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	AND (@SiteCode is Null or a.LabSiteCode = @SiteCode)
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
	ORDER BY a.OrderNumber

END
GO
/****** Object:  StoredProcedure [dbo].[GetLastFrameOfChoiceDateByIndividualID]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
/* Modified:
Kevin Bush - 12 Mar 15 - 
	I added an extra component to the where statement that will include all on hold orders with the on hold 
	for confirmation flag equal to false.
	4/30/15 - baf - updated sp to correctly calculate dates and include legacy orders
	12/15/15 - baf - modified to use revised FOC logic
*/
-- =============================================
CREATE PROCEDURE [dbo].[GetLastFrameOfChoiceDateByIndividualID] 
@IndividualID	INT,
@LastOrderDate	DATETIME OUTPUT,
@LastFocOrder	VARCHAR(16) OUTPUT,
@FrameCode	VarChar(6) OUTPUT,
@Justify		bit Output,
@NextFOCDate	DateTime Output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	Select @LastOrderDate = b.OrderCreateDate, @FrameCode = FrameCode, @NextFOCDate = NextFOCDate, @Justify = Justify
	From (
	Select Row_Number() over (Partition by IndividualID_Patient order by NextFOCDate desc) as RowNum, 
		OrderNumber, IndividualID_Patient, NextFOCDate, FrameCode,OrderCreateDate,
		Case when  NextFOCDate > GETDATE() then 1 else 0 end Justify
	From
	(
		Select distinct po.OrderNumber,po.Demographic, pos.OrderStatusTypeID, IndividualID_Patient, po.DateLastModified,
			 FrameNotes, f.FrameCode, pol.DateLastModified as OrderCreateDate, po.LegacyID, i.nextFOCDate,
			po.ModifiedBy
		from dbo.PatientOrder po
			INNER JOIN dbo.Individual i ON i.ID = po.IndividualID_Patient
			inner Join dbo.Frame f on po.FrameCode = f.FrameCode
			Inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
			Left Join dbo.PatientOrderStatus pol on pol.OrderNumber = po.OrderNumber
		Where pos.IsActive = 1 and pos.OrderStatusTypeID IN (1,3,5,6,9,15) and
			f.IsFOC = 1 AND pol.OrderStatusTypeID in (1,6)  and po.IndividualID_Patient = @IndividualID
	) a
) b
Where RowNum = 1

END
GO

/****** Object:  StoredProcedure [dbo].[GetLabOrdersForDispenseCheckInByLabCode]    Script Date: 10/10/2019 1:04:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 27 Mar 15
-- Description:	This will get all orders for the lab to either check in or dispense.
-- Modified:  baf - 4/15/15 to allow for multiple statuses ready for check-in, not
--		just 1 - Clinic Created.
--		7/7/2017 - baf - added StatusCode 5 - Hold For Stock
--		12/21/2017 - baf - modified to use po.DateLastModified instead of pos.datelastmodified
--		9/18/2019 - baf - added Priority for Story FS-227
-- =============================================
ALTER PROCEDURE [dbo].[GetLabOrdersForDispenseCheckInByLabCode] 
@LabSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
--,
--@ForDispense	BIT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	------------ Audit MultiRead 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrderStatus t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber 
	INNER JOIN dbo.Individual b ON a.IndividualID_Patient = b.ID 
	WHERE t.IsActive = 1 
	AND t.OrderStatusTypeID in (1,2) 
	AND a.LabSiteCode = @LabSiteCode
	AND a.IsActive = 1
	ORDER BY t.DateLastModified, a.OrderNumber                                                  
	
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers To Dispense: ') + a.OrderNumber  + ', ' 
	FROM  dbo.PatientOrderStatus t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber 
	INNER JOIN dbo.Individual b ON a.IndividualID_Patient = b.ID 
	WHERE t.IsActive = 1
	AND t.OrderStatusTypeID in (1,2,5) 
	AND a.LabSiteCode = @LabSiteCode
	AND a.IsActive = 1
	ORDER BY t.DateLastModified, a.OrderNumber
	
	IF @Notes IS NOT NULL AND @PatientList IS NOT null
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	-------------------------------------------
	
	DECLARE @status	VARCHAR(2);
	--IF @ForDispense = 1 

	Begin
		SELECT po.OrderNumber,
		po.ClinicSiteCode,
		po.ShipToPatient, --  Added 9/17/14 for Lab Mailing to Patient - BAF
		po.ShipAddress1,
		po.ShipAddress2,
		po.ShipAddress3,
		po.ShipCity, 
		po.ShipState,
		po.ShipCountry, 
		po.ShipZipCode,
		pos.LabSiteCode,
		po.DateLastModified, 
		pos.OrderStatusTypeID, 
		pos.StatusComment, 
		pos.ModifiedBy,
		po.FrameCode,
		po.LensType,
		po.LensMaterial, 
		i.ID AS IndividualID, -- added 6/8/17
		i.LastName,
		i.MiddleName,
		i.FirstName,
		Right(po.Demographic,1) as Priority
		FROM PatientOrder po
		INNER JOIN PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
		INNER JOIN Individual i on po.IndividualID_Patient = i.ID
		WHERE 
		pos.IsActive = 1 and --(pos.OrderStatusTypeID in (1, 9, 4, 6, 10) or
			(pos.OrderStatusTypeID in (1,2,4,5,6, 9,10)) and--OR (pos.OrderStatusTypeID = 1 AND StatusComment LIKE '%Created%')) and
		--pos.IsActive = 1 and Cast(pos.OrderStatusTypeID as Varchar(2)) in (@status) and 
		po.LabSiteCode = @LabSiteCode AND
		po.IsActive = 1 and i.IsActive = 1
		ORDER BY ORderStatusTypeID, [Priority], pos.DateLastModified, po.OrderNumber;
	END
END
GO
/****** Object:  StoredProcedure [dbo].[GetInserts]    Script Date: 10/03/2019 13:59:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/26/2013
-- Description:	This stored procedure retrieves all data necessary
--		to complete the Inserts portion of the 
--		Summary for All Orders Processed (Rpt 54).
-- =============================================
CREATE PROCEDURE [dbo].[GetInserts]
	-- Add the parameters for the stored procedure here
	@FromDate datetime,
	@ToDate datetime,
	@LabSiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select LabSiteCode, FrameCode, SUM(NAD) as TNAD, SUM(NRet) as TNRET, 
	SUM(MAD) as TMAD, SUM(MRet) as TMRet, SUM(AAD) as TAAD, SUM(ARet) as TARet,
	SUM(FAD) as TFAD, sum(FRet) as TFRet, SUM(PDRL) as TPDRL, SUM(FmrPOW) as TPOW,
	SUM(CAD) as TCAD, SUM(PAD) as TPAD, SUM(BAD) as TBAD, SUM(FRes) as TFRES, 
	SUM(ARes) as TARes, SUM(ANG) as TANG, sum(FNG) as TFNG,SUM(NRes) as TNRes,
	SUM(NDepAD) as TNDep, SUM(MRes) as TMRes, SUM(DodRemCONUS) as TDRCONUS, 
	SUM(VABen) as TVABen, SUM(IMET) as TIMET, SUM(other) as TOther
from(
	select LabSiteCode, FrameCode,
		case when a.bos = 'N' and statuscode in (11,14) then Pairs else 0 end NAD,
		case when a.bos = 'N' and statuscode = 31 then Pairs else 0 end NRet,
		case when a.bos = 'M' and statuscode = 11 then Pairs else 0 end MAD,
		case when a.bos = 'M' and statuscode = 31 then Pairs else 0 end MRet,
		case when a.bos = 'A' and statuscode in (11,14) then Pairs else 0 end AAD,
		case when a.bos = 'A' and statuscode = 31 then Pairs else 0 end ARet,
		case when a.bos = 'F' and statuscode in (11,14) then Pairs else 0 end FAD,
		case when a.bos = 'F' and statuscode = 31 then Pairs else 0 end FRet,
		case when statuscode = 32 then Pairs else 0 end PDRL,
		case when statuscode = 36 then Pairs else 0 end FmrPOW,
		case when a.bos = 'C' and statuscode = 11 then Pairs else 0 end CAD,
		case when a.bos = 'P' and statuscode = 11 then Pairs else 0 end PAD,
		case when a.bos = 'B' and statuscode = 11 then Pairs else 0 end BAD,
		case when a.bos = 'F' and statuscode = 12 then Pairs else 0 end FRes,
		case when a.bos = 'A' and statuscode = 12 then Pairs else 0 end ARes,
		case when a.bos = 'A' and statuscode = 15 then Pairs else 0 end ANG,
		case when a.bos = 'F' and statuscode = 15 then Pairs else 0 end FNG,
		case when a.bos = 'N' and statuscode = 12 then Pairs else 0 end NRes,
		case when a.bos = 'N' and statuscode = 41 then Pairs else 0 end NDepAD,
		case when a.bos = 'M' and statuscode = 12 then Pairs else 0 end MRes,
		case when a.bos = 'K' and statuscode = 55 then Pairs else 0 end DodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 then Pairs else 0 end VABen,
		case when a.bos = 'K' and statuscode = 71 then Pairs else 0 end IMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) then Pairs else 0 end Other
	from
	(
		select pos.OrderStatusTypeID, po.ClinicSiteCode, po.LabSiteCode,
			REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(i.Demographic, 5,2) AS statuscode,
			SUBSTRING(i.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(i.Demographic, 4, 1) AS bos, po.FrameCode, po.LensType,
			po.Pairs, f.FrameDescription, f.FrameNotes,
			case when f.FrameNotes like '%FOC%' THEN 1 ELSE 0 END IsFOC,
			F.IsInsert, IsMultivision
		from dbo.PatientOrder po left join dbo.PatientOrderStatus pos
		on po.OrderNumber = pos.OrderNumber
		inner join Individual i on i.ID = po.IndividualID_Patient
		inner join Frame f on f.FrameCode = po.FrameCode
		where poS.DateLastModified between @FromDate and @ToDate and 
		pos.OrderStatusTypeID > 2 and pos.OrderStatusTypeID not in (3,4,5)
		and FrameNotes not in ('JUSTIFY', 'FOC') AND 
		IsInsert = 1 and pos.IsActive = 1
	) a
)b
where LabSiteCode = @LabSiteCode
Group by LabSiteCode, FrameCode
order by LabSiteCode, FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetFOCFrames]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/26/2013
-- Description:	This stored procedure retrieves all data necessary
--		to complete the FOC Frame portion of the 
--		Summary for All Orders Processed (Rpt 54).
-- =============================================
CREATE PROCEDURE [dbo].[GetFOCFrames]
	@FromDate Datetime,
	@ToDate Datetime,
	@LabSiteCode Varchar (6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select LabSiteCode, FrameCode, SUM(NAD) as TNAD, SUM(NRet) as TNRET, 
	SUM(MAD) as TMAD, SUM(MRet) as TMRet, SUM(AAD) as TAAD, SUM(ARet) as TARet,
	SUM(FAD) as TFAD, sum(FRet) as TFRet, SUM(PDRL) as TPDRL, SUM(FmrPOW) as TPOW,
	SUM(CAD) as TCAD, SUM(PAD) as TPAD, SUM(BAD) as TBAD, SUM(FRes) as TFRES, 
	SUM(ARes) as TARes, SUM(ANG) as TANG, sum(FNG) as TFNG,SUM(NRes) as TNRes,
	SUM(NDepAD) as TNDep, SUM(MRes) as TMRes, SUM(DodRemCONUS) as TDRCONUS, 
	SUM(VABen) as TVABen, SUM(IMET) as TIMET, SUM(Other) as TOther
from
(
	select LabSiteCode, FrameCode,
		case when a.bos = 'N' and statuscode in (11,14) then Pairs else 0 end NAD,
		case when a.bos = 'N' and statuscode = 31 then Pairs else 0 end NRet,
		case when a.bos = 'M' and statuscode = 11 then Pairs else 0 end MAD,
		case when a.bos = 'M' and statuscode = 31 then Pairs else 0 end MRet,
		case when a.bos = 'A' and statuscode in (11,14) then Pairs else 0 end AAD,
		case when a.bos = 'A' and statuscode = 31 then Pairs else 0 end ARet,
		case when a.bos = 'F' and statuscode in (11,14) then Pairs else 0 end FAD,
		case when a.bos = 'F' and statuscode = 31 then Pairs else 0 end FRet,
		case when statuscode = 32 then Pairs else 0 end PDRL,
		case when statuscode = 36 then Pairs else 0 end FmrPOW,
		case when a.bos = 'C' and statuscode = 11 then Pairs else 0 end CAD,
		case when a.bos = 'P' and statuscode = 11 then Pairs else 0 end PAD,
		case when a.bos = 'B' and statuscode = 11 then Pairs else 0 end BAD,
		case when a.bos = 'F' and statuscode = 12 then Pairs else 0 end FRes,
		case when a.bos = 'A' and statuscode = 12 then Pairs else 0 end ARes,
		case when a.bos = 'A' and statuscode = 15 then Pairs else 0 end ANG,
		case when a.bos = 'F' and statuscode = 15 then Pairs else 0 end FNG,
		case when a.bos = 'N' and statuscode = 12 then Pairs else 0 end NRes,
		case when a.bos = 'N' and statuscode = 41 then Pairs else 0 end NDepAD,
		case when a.bos = 'M' and statuscode = 12 then Pairs else 0 end MRes,
		case when a.bos = 'K' and statuscode = 55 then Pairs else 0 end DodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 then Pairs else 0 end VABen,
		case when a.bos = 'K' and statuscode = 71 then Pairs else 0 end IMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) then Pairs else 0 end Other
	from
	(
		select pos.OrderStatusTypeID, po.ClinicSiteCode, po.LabSiteCode,
			REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(i.Demographic, 5,2) AS statuscode,
			SUBSTRING(i.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(i.Demographic, 4, 1) AS bos, po.FrameCode, po.LensType,
			po.Pairs, f.FrameDescription, f.FrameNotes, IsFOC,
			--case when f.FrameNotes like '%FOC%' THEN 1 ELSE 0 END IsFOC,
			F.IsInsert, IsMultivision
		from dbo.PatientOrder po left join dbo.PatientOrderStatus pos
		on po.OrderNumber = pos.OrderNumber
		inner join Individual i on i.ID = po.IndividualID_Patient
		inner join Frame f on f.FrameCode = po.FrameCode
		where poS.DateLastModified between @FromDate and @ToDate and 
		pos.OrderStatusTypeID > 2 and pos.OrderStatusTypeID not in (3,4,5)
		and IsFOC = 1 and --FrameNotes like  ('FOC') AND 
		IsInsert = 0 and pos.IsActive = 1 and pos.LabSiteCode = @LabSiteCode
	) a
)b
Group by LabSiteCode, FrameCode
order by LabSiteCode, FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetClinicSummaryReport]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
 Author:		Barb Fieldhausen
 Create date: 21 October 2013
 Description:	This procedure pulls the data for the
				Clinic Summary Report
 Modified:  2/11/15 BAF - Changed check for 'Clear' lenses
				to po.Tint <> 'CLR' and po.Tint <> 'CL'
			1/4/18 baf - Added Coatings
 ============================================= */
CREATE PROCEDURE [dbo].[GetClinicSummaryReport]
	@ToDate DateTime,
	@FromDate DateTime,
	@SiteCode	VARCHAR(6) = null,
	@Priority VarChar(1) = null,
	@Status VarChar(2) = null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
Select ClinicSiteCode,LabSiteCode,FrameCode, SUM(OrderCnt) as TotOrders,SUM(Pairs) as TotPairs, 
	SUM(NumberOfCases) as TotCases, Sum(Cast(Tinted as int)) as TotTint, SUM(CAST(Coated AS INT)) AS Coated,
	SUM(Cast(IsMulti as Int)) as TotMulti, SUM(CAST(IsSingle as Int)) as TotSingle, 
	SUM(Cast(IsFOC as Int)) as TotFOC, SUM(Cast(IsInsert as Int)) as TotInsert, 
	StatusCode, StatusType,SiteName
From
(
	Select po.OrderNumber, COUNT(po.OrderNumber) as OrderCnt, po.FrameCode, SUBSTRING(po.demographic,5,2) as StatusCode, 
		IsInsert, Case when po.IsMultivision = 1 then 1 else 0 end IsMulti, 
		Case when po.IsMultivision = 0 then 1 else 0 end IsSingle, 
		case when fr.FrameNotes like '%FOC%' and fr.FrameNotes not like '%MULTIFOCAL%' then 1 else 0 end IsFOC,
		Case when (po.Tint <> 'CLR') and (po.Tint <> 'CL') then 1 else 0 end Tinted,
		CASE WHEN (po.Coatings <> '' OR po.Coatings IS NOT NULL) THEN 1 ELSE 0 END Coated,
		pos.LabSiteCode,ClinicSiteCode, NumberOfCases, Pairs, sc.SiteName,lt.[Text] as StatusType
	From dbo.PatientOrder po
		Inner Join dbo.Frame fr on fr.FrameCode = po.FrameCode
		Inner Join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
		inner join dbo.LookupTable lt on Value = SUBSTRING(po.demographic,5,2)
		inner join dbo.SiteCode sc on sc.SiteCode = pos.LabSiteCode
	where po.ClinicSiteCode = @SiteCode and
		Convert(VarChar(10),po.DateLastModified,101) between @FromDate and DATEADD(d,+1,@ToDate) and
		pos.OrderStatusTypeID not in (5,9,10,14,15,16,17) and
		pos.IsActive = 1 and
		lt.Code = 'PatientStatusType'
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) and
		SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
	Group by po.FrameCode, SUBSTRING(po.Demographic,5,2), IsInsert, po.IsMultivision, fr.FrameNotes,
		pos.LabSiteCode, ClinicSiteCode,NumberOfCases, Pairs, SiteName ,lt.[Text], Tint, po.OrderNumber, Coatings
) a

Group by LabSiteCode, SiteName, FrameCode, StatusCode, ClinicSiteCode, StatusType
Order by FrameCode

END
GO
/****** Object:  StoredProcedure [dbo].[Get711DataByOrderNumber]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- MODIFIED:  2/23/2017 - baf - added Dispense Comments
--			1/3/2018 - baf - Added Coatings		
--		9/6/2018 - baf - Added comments for Lab Redirection per
--			Story 383
-- =============================================
CREATE PROCEDURE [dbo].[Get711DataByOrderNumber] 
@OrderNumber	VARCHAR(16),
@ModifiedBy		VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	---- Auditing Single Read 10/15/14 - BAF
		Declare @ReadDate DateTime = GetDate()
		Declare @PatientID Int
					
		Select  @PatientID = a.IndividualID_Patient 
		FROM dbo.PatientOrderStatus t
		INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
		INNER JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
		INNER JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
		INNER JOIN dbo.IndividualIDNumber h ON a.IndividualID_Patient = h.IndividualID
		Left JOIN dbo.IndividualPhoneNumber i ON a.PatientPhoneID = i.IndividualID
		INNER JOIN dbo.SiteCode k ON a.ClinicSiteCode = k.SiteCode
		INNER JOIN dbo.SiteCode l ON a.LabSiteCode = l.SiteCode
		inner join dbo.SiteAddress sac on a.ClinicSiteCode = sac.SiteCode
		inner join dbo.SiteAddress sal on a.LabSiteCode = sal.SiteCode
		LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
		WHERE a.OrderNumber = @OrderNumber
			and a.IsActive = 1
			and sac.AddressType = 'MAIL'
			and sal.AddressType = 'MAIL'
			
		
		Exec InsertAuditSingleRead @ReadDate, @PatientID, 20, @ModifiedBy, @OrderNumber
	-----------------------
	
	Declare @IDCnt int = 1, @IDNbr VarChar(10), @PatID int
	
	Select @PatID = IndividualID_Patient from dbo.PatientOrder
	where OrderNumber = @OrderNumber
	
	Select @IDCnt = COUNT(IDNumber) from dbo.IndividualIDNumber 
	where IndividualID = @PatID
	
	IF @IDCnt = 1
		Begin
			Select @IDNbr = IDNumber from dbo.IndividualIDNumber
			where IndividualID = @PatID --and IDNumberType = 'SSN'
		End
	ELSE
		Begin
			Select @IDNbr = IDNumber from dbo.IndividualIDNumber
			where IndividualID = @PatID and IDNumberType = 'DIN' and ISActive = 1
		End
	---------------------------
	-- Below added for Story 383-Ensure Redirected comment on 771
	Create Table #OrderPos (RowID int, OrderNumber VarChar(16), OrderStatus Int, Comment VarChar(300),
		DateModified DateTime)
	Insert into #OrderPos
	SELECT ROW_NUMBER() OVER(ORDER BY DateLastModified DESC) AS Row#,		OrderNumber, OrderStatusTypeID, StatusComment, DateLastModified 	FROM dbo.PatientOrderStatus pos	WHERE pos.OrderNumber = @OrderNumber
	
	Declare @Comment VarChar(300) = Null, @RowID int
	Select @RowID = RowID from #OrderPos
	where OrderStatus = 4 -- and RowID < 2

	If @RowID <= 2
		Begin
			Select @Comment = Comment from #OrderPOS where RowID = @RowID
		End
	ELSE
		Begin
			Set @Comment = ''
		ENd
			
	-------------------------------------------------
	SELECT distinct a.OrderNumber,
		SUBSTRING(a.Demographic, 7, 1) AS sexcode,
		REPLACE(SUBSTRING(a.Demographic, 1, 3), '*', '') AS rankcode,
		SUBSTRING(a.Demographic, 5,2) AS statuscode,
		SUBSTRING(a.Demographic, 8, 1) AS orderpriority,
		SUBSTRING(a.Demographic, 4, 1) AS bos,
		SUBSTRING(f.FirstName, 1,1) + SUBSTRING(f.MiddleName, 1,1) + SUBSTRING(f.LastName,1,1) AS techinitials,
		a.LensType,
		a.LensMaterial,
		a.Tint,
		a.Coatings,		-- Added 1/3/18
		a.ODSegHeight,
		a.OSSegHeight,
		a.NumberOfCases,
		a.Pairs,
		a.FrameCode,
		a.FrameColor,
		a.FrameEyeSize,
		a.FrameBridgeSize,
		a.FrameTempleType,
		b.ODPDDistant,
		b.OSPDDistant,
		b.PDDistant,
		b.ODPDNear,
		b.OSPDNear,
		b.PDNear,
		a.ClinicSiteCode,
		k.SiteName AS clinicname,
		sac.Address1 AS clinicaddress1,
		sac.Address2 AS clinicaddress2,
		sac.Address3 as clinicaddress3,
		sac.City AS cliniccity,
		sac.Country AS cliniccountry,
		sac.State AS clinicstate,
		sac.ZipCode AS cliniczipcode,
		a.LabSiteCode,
		l.SiteName AS labname,
		sal.Address1 AS labaddress1,
		sal.Address2 AS labaddress2,
		sal.Address3 as labaddress3,
		sal.City AS labcity,
		sal.Country AS labcountry,
		sal.State AS labstate,
		sal.ZipCode AS labzipcode,
		a.ShipToPatient,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipCity,
		a.ShipState,
		a.ShipZipCode,
		a.ShipAddressType,
		a.LocationCode,
		-- Below Case added for Story 383
		Case when @Comment = '' then
			t.StatusComment + '.  ' + a.UserComment1 --as UserComment1,
		else @Comment + '.    ' + a.UserComment1 end UserComment1,
		a.UserComment2 + '.   ' + d.Comments as UserComment2,
        a.IsGEyes,
        a.IsMultivision,
        a.VerifiedBy,
        a.PatientEmail,
        a.OnholdForConfirmation,
        b.ODAdd,
        b.ODAxis,
        b.ODCylinder,
        b.ODHBase,
        b.ODHPrism,
        b.ODSphere,
        b.ODVBase,
        b.ODVPrism,
        b.OSAdd,
        b.OSAxis,
        b.OSCylinder,
        b.OSHBase,
        b.OSHPrism,
        b.OSSphere,
        b.OSVBase,
        b.OSVPrism,
        c.ODCorrectedAcuity,
        c.ODOSCorrectedAcuity,
        c.ODOSUncorrectedAcuity,
        c.ODUncorrectedAcuity,
        c.OSCorrectedAcuity,
        c.OSUncorrectedAcuity,
        d.FirstName,
        d.MiddleName,
        d.LastName,
        g.LastName + ', ' + g.FirstName AS doctor,
        c.ExamDate,
        @IDNbr as patientidnumber,
         i.AreaCode + i.PhoneNumber as patientphonenumber,
        a.DateLastModified AS dateordercreated, 
        a.OSDistantDecenter as osdistantdecenter,
        a.ODDistantDecenter as oddistantdecenter,
        a.OSNearDecenter as osneardecenter,
        a.ODNearDecenter as odneardecenter,
        a.ODBase as odbase,
        a.OSBase as osbase, 
        a.ONBarCode,
        a.DispenseComments
    FROM dbo.PatientOrderStatus t
	INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber
	INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
	INNER JOIN dbo.Individual d ON a.IndividualID_Patient = d.ID
	Left JOIN dbo.Individual f ON a.IndividualID_Tech = f.ID
	Left JOIN dbo.Individual g ON b.IndividualID_Doctor = g.ID
	Left JOIN dbo.IndividualPhoneNumber i ON a.PatientPhoneID = i.ID
	INNER JOIN dbo.SiteCode k ON a.ClinicSiteCode = k.SiteCode
	INNER JOIN dbo.SiteCode l ON a.LabSiteCode = l.SiteCode
	inner join dbo.SiteAddress sac on a.ClinicSiteCode = sac.SiteCode
	inner join dbo.SiteAddress sal on a.LabSiteCode = sal.SiteCode
	LEFT OUTER JOIN dbo.Exam c ON b.ExamID = c.ID
	WHERE a.OrderNumber = @OrderNumber
		and a.IsActive = 1 
		and sac.AddressType = 'MAIL'
		and sal.AddressType = 'MAIL'
		and t.IsActive = 1
		
	Drop Table #OrderPos	
END
GO
/****** Object:  StoredProcedure [dbo].[Get711DataByLabCode]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified: 3/31/2015 - baf - Modified Order Number to VarCharMax
--		to enable multiple order numbers to be passed, then parse and
--		loop through to generate the DD Form 771(s).
--		Now works based upon order numbers passed, not order status and
--		SiteCode.
-- 		8/7/2015 - baf - Added TBI Tint Desc
--		3/4/2016 - baf - Commenting out,for now, the use of DODID instead of SSN
--	11/17/2016 - baf - Modified to select DODID when available or use SSN
--	02/23/2017 - baf - Added DispenseComments
--  01/03/2018 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[Get711DataByLabCode] 
@SiteCode	VARCHAR(6) = Null,
@OrderNbr VARCHAR(MAX) = Null,
@ModifiedBy	VarChar(200)

AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
		Declare @OrderNum VarChar(Max),
				@RecCnt INT,
				@CurRec INT = 1,
				@Order	VarChar(16);
		
		CREATE TABLE #tmpOrderNbrs771
		(RowID int, OrderNbr VarChar(16), PatientID int, DateModified DateTime,
		 IDCnt int, IDNbr VarChar(10), StatRowID int, StatComment VarChar(300))
		 
		 Create Table #OrderPos (RowID int, OrderNumber VarChar(16), OrderStatus Int, Comment VarChar(300),
		 DateModified DateTime)

		If RIGHT(@OrderNbr,1) = ',' 
		Begin
			Set @OrderNum = LEFT(@OrderNbr,LEN(@OrderNbr) -1)
		
			INSERT INTO #tmpOrderNbrs771 (RowID, OrderNbr)
			SELECT * FROM dbo.SplitStrings(@OrderNum,',')	
				
			Select @RecCnt = MAX(RowID) FROM #tmpOrderNbrs771;
		End
		ELSE if @OrderNbr is not null
		BEGIN
			Set @OrderNum = @OrderNbr

			INSERT INTO #tmpOrderNbrs771 (RowID, OrderNbr)
			SELECT * FROM dbo.SplitStrings(@OrderNum,',')	
		END
		
		Update #tmpOrderNbrs771 Set PatientID = (Select IndividualID_Patient from dbo.PatientOrder po 
		where po.OrderNumber = #tmpOrderNbrs771.OrderNbr), DateModified = (Select DateLastModified 
		from dbo.PatientOrderStatus pos where pos.OrderNumber = #tmpOrderNbrs771.OrderNbr and pos.IsActive = 1)
		
		Update #tmpOrderNbrs771 set IDCnt = (Select COUNT(IDNumber) from dbo.IndividualIDNumber idn
		where #tmpOrderNbrs771.PatientID = idn.IndividualID)

		Update #tmpOrderNbrs771 set StatRowID = 0, StatComment = '' 

		--Select * from #tmpOrderNbrs771-- Remove after testing
		
		Declare @IDCnt int, @PatID int
		
		Declare Stat Cursor
		For Select IDCnt, PatientID From #tmpOrderNbrs771
		
		Open Stat
		Fetch Stat into @IDCnt, @PatID
		While (@@FETCH_STATUS = 0)
			Begin
				If @IDCnt = 1
					Begin
						Update #tmpOrderNbrs771 set IDNbr = (Select IDNumber from dbo.IndividualIDNumber idn
							where IndividualID = @PatID) -- and IDNumberType = 'SSN' and IsActive = 1)
						where #tmpOrderNbrs771.PatientID = @PatID
					END
				ELSE
					BEGIN
						Update #tmpOrderNbrs771 set IDNbr = (Select IDNumber from dbo.IndividualIDNumber idn
							where IndividualID = @PatID and IDNumberType = 'DIN' and IsActive = 1)
						where #tmpOrderNbrs771.PatientID = @PatID 
					end
				Fetch Stat into @IDCnt, @PatID		
			END
			Close Stat
			Deallocate Stat
		
		Select @RecCnt = COUNT(*) FROM #tmpOrderNbrs771
		Set @CurRec = 1
		Declare @iRow as int, @iOrder as VarChar(16), @iComment VarChar(300)

				
		While @CurRec <= @RecCnt
			Begin
				Delete from #OrderPos
							
				Insert into #OrderPos
				SELECT ROW_NUMBER() OVER(ORDER BY DateLastModified DESC) AS Row#,					OrderNumber, OrderStatusTypeID, StatusComment, DateLastModified 				FROM dbo.PatientOrderStatus pos				WHERE pos.OrderNumber = (Select OrderNbr from #tmpOrderNbrs771
					where RowID = @CurRec)
					
				Set @iRow = 0
				Set @iOrder = ''
				Set @iComment = ''
				
				Select @iOrder = OrderNbr from #tmpOrderNbrs771 where RowID = @CurRec
				
				Update #tmpOrderNbrs771 set StatRowID= @iRow, StatComment = '' where OrderNbr = @iOrder

				Select @iRow = RowID, @iComment = Comment from #OrderPos where OrderStatus = 4 and OrderNumber = @iOrder
				
	--			Print CAST(@CurRec as VarChar(5)) + ' - ' + @iOrder + ', ' + CAST(@iRow as VarChar(5)) + ', ' + @iComment -- Remove after testing
			
				if @iRow < 3
				Begin					
					Update #tmpOrderNbrs771 set StatRowID = @iRow, 
						StatComment = (Select Comment from #OrderPos 
					where OrderNbr = @iOrder  and RowID = @iRow) where OrderNbr = @iOrder
				End
				Else
				Begin
					Update #tmpOrderNbrs771 set StatRowID = 0, 
						StatComment = '' where OrderNbr = @iOrder 
				End
					
				Set @CurRec = @CurRec + 1
			End
		
		If @OrderNbr is null
		Begin
			Insert into #tmpOrderNbrs771 (OrderNbr, PatientID, DateModified)
		 	SELECT distinct a.OrderNumber, IndividualID_Patient, t.DateLastModified as DateLastModified
			FROM 
			 dbo.PatientOrderStatus t
			INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber
			WHERE t.OrderStatusTypeID in (1,4,6,9,10) and a.IsActive = 1 
			AND a.IsGEyes = 0
			AND a.LabSiteCode = @SiteCode
			and t.IsActive = 1
			GROUP BY a.LabSiteCode, IndividualID_Patient, a.OrderNumber, t.DateLastModified
		END
			
		Declare @MaxDate as DateTime,
			@MinDate as DateTime
			
		Select @MaxDate = MAX(DateModified) from #tmpOrderNbrs771
		Select @MinDate = MIN(DateModified) from #tmpOrderNbrs771

	SELECT DISTINCT a.OrderNumber,
		SUBSTRING(a.Demographic, 7, 1) AS sexcode,
		SUBSTRING(a.Demographic, 1, 3) AS rankcode,
		SUBSTRING(a.Demographic, 5,2) AS statuscode,
		SUBSTRING(a.Demographic, 8, 1) AS orderpriority,
		SUBSTRING(a.Demographic, 4, 1) AS bos,
		SUBSTRING(m.FirstName, 1,1) + SUBSTRING(m.MiddleName, 1,1) + SUBSTRING(m.LastName,1,1) AS techinitials,
		a.LensType, 		a.LensMaterial,		a.Tint,	isnull(a.Coatings,'') as Coatings,	a.ODSegHeight,		a.OSSegHeight,		a.NumberOfCases,
		a.Pairs,		a.FrameCode,		a.FrameColor,		a.FrameEyeSize,		a.FrameBridgeSize,		a.FrameTempleType,
		g.ODPDDistant,		g.OSPDDistant,		g.PDDistant,		g.ODPDNear,		g.OSPDNear,		g.PDNear,		a.ClinicSiteCode,
		c.SiteName AS clinicname,		e.Address1 AS clinicaddress1,		e.Address2 AS clinicaddress2,		e.Address3 as clinicaddress3,
		e.City AS cliniccity,
		e.Country AS cliniccountry,
		e.State AS clinicstate,
		e.ZipCode AS cliniczipcode,
		b.LabSiteCode,
		d.SiteName AS labname,
		f.Address1 AS labaddress1,
		f.Address2 AS labaddress2,
		f.Address3 as labaddress3,
		f.City AS labcity,
		f.Country AS labcountry,
		f.State AS labstate,
		f.ZipCode AS labzipcode,
		a.ShipToPatient,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipAddress3,
		a.ShipCity,
		a.ShipState,
		case when LEN(a.ShipZipCode) > 5 then LEFT(a.ShipZipCode,5) end BaseZip,
		case when LEN(a.shipZipCode) > 5 then RIGHT(a.ShipZipCode,4) 
			else '0000'end ZipPlus,
		a.ShipZipCode,
		a.ShipAddressType,
		a.LocationCode,
		Case when y.StatRowID <> 0 then
			y.StatComment + '.     ' + a.UserComment1
		else
			b.StatusComment + '.  ' + a.UserComment1 end UserComment1,
		a.UserComment2 + '.   ' + i.Comments as UserComment2,
        a.IsGEyes,
        a.IsMultivision,
        a.VerifiedBy,
        a.PatientEmail,
        a.DateLastModified AS dateordercreated,
        a.OSDistantDecenter as osdistantdecenter,
        a.ODDistantDecenter as oddistantdecenter,
        a.OSNearDecenter as osneardecenter,
        a.ODNearDecenter as odneardecenter,
        a.ODBase as odbase,
        a.OSBase as osbase,
        g.ODAdd, 
        RIGHT('000' + CAST(g.ODAxis AS VarCHAR(3)),3) AS ODAxis,  -- Modified 4/1/15 - baf per user request
        convert(decimal(4,2),round(g.ODCylinder/25.0,2) * 25) as ODCylinder,
        g.ODHBase,
        g.ODHPrism,
        g.ODSphere,
        g.ODVBase,
        g.ODVPrism,
        g.OSAdd,
        RIGHT('000' + CAST(g.OSAxis AS VARCHAR(3)),3) AS OSAxis,  -- Modified 4/1/15 - baf per user request
		convert(decimal(4,2),round(g.OSCylinder/25.0,2) * 25) as  OSCylinder,
        g.OSHBase,
        g.OSHPrism,
        g.OSSphere,
        g.OSVBase,
        g.OSVPrism,
        h.ODCorrectedAcuity,
        h.ODOSCorrectedAcuity,
        h.ODOSUncorrectedAcuity,
        h.ODUncorrectedAcuity,
        h.OSCorrectedAcuity,
        h.OSUncorrectedAcuity,
        h.examdate,
        i.FirstName,
        i.MiddleName,
        i.LastName,
        j.LastName + ', ' + j.FirstName AS doctor,
		y.IDNbr as patientidnumber,
        l.AreaCode + l.PhoneNumber as patientphonenumber,
        n.[Text] as BOSDesc,
        o.[text] as StatDesc,
        p.[Text] as OrdPriDesc,
        case
			when q.[Value] = '15' then 'N-15 Drk Grey'
			when q.[Value] = '31' then 'N-31 Med Grey'
			when q.[Value] = '40' then 'Nom. 60% Trans'
	--		when q.[Value] = 'AR' then 'Anti-Refl'	-- 1/3/18 baf
			when q.[Value] = 'CL' then 'Clear'
			when q.[Value] = 'S1' then 'S-1 Lt Pink'
			when q.[Value] = 'S2' then 'S-2 Med Pink'
	--		when q.[Value] = 'UV' then 'UV-400'		-- 1/3/18 baf
			WHEN q.[Value] = '60' THEN 'Nom. 40% Trans'
			WHEN q.[Value] = 'P8' THEN 'Pink 85%'
			WHEN q.[Value] = 'P6' THEN 'Pink 60%'
			WHEN q.[Value] = 'A8' THEN 'Amber 80%'
			WHEN q.[Value] = 'A6' THEN 'Amber 60%'
			WHEN q.[Value] = 'B8' THEN 'Blue 85%'
			WHEN q.[Value] = 'B6' THEN 'Blue 65%'
			WHEN q.[Value] = 'FL' THEN 'FL41 50% (Rose)'
        end TintDesc,
        CASE	-- Added 1/3/18
			WHEN fi.Value = 'AR' THEN 'Anti-Refl'
			WHEN fi.Value = 'UV' THEN 'UV-400'
			WHEN fi.value = 'AR/UV' THEN 'Anti-Refl/UV-400'
			WHEN fi.value = 'UV/AR' THEN 'UV-400/Anti-Refl'
			else '' -- added jp 2/15/18
		END CoatingDesc,
        r.[Text] as ColorDesc,
        s.[Text] as MaterialDesc,
        t.[text] as LensDesc,
        a.ONBarCode,
        a.DispenseComments -- added 2/23/17
    FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    Left JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	left outer JOIN dbo.FrameItem fi ON a.Coatings = fi.Value
	Inner Join #tmpOrderNbrs771 y on y.PatientID = a.IndividualID_Patient
	WHERE a.IsActive = 1
	AND b.IsActive = 1
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	--AND fi.TypeEntry = 'Coating'  -- jp 2/15/18
	AND (@SiteCode is Null or b.LabSiteCode = @SiteCode)
	AND (a.OrderNumber IN (SELECT OrderNbr FROM #tmpOrderNbrs771))
	ORDER BY a.ClinicSiteCode, a.OrderNumber;

	--Select * from #tmpOrderNbrs771
	--Select * from #OrderPos

	DROP TABLE #tmpOrderNbrs771
	Drop table #OrderPos
End
GO
/****** Object:  StoredProcedure [dbo].[GetClinicProductionOrders]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 11 October 2013
-- Description:	This stored procedure retrieves the
--		number of orders and pairs for a specified
--		site during the specified time frame.
-- =============================================
CREATE PROCEDURE [dbo].[GetClinicProductionOrders]
	-- Add the parameters for the stored procedure here
	@FromDate DateTime,
	@ToDate DateTime,
	@SiteCode VARCHAR(6) = null,
	@Status varchar(2) = null,
	@Priority varchar(1) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

select a.ClinicSiteCode,sc.SiteName, b.Address1, b.Address2, b.Address3, b.City, b.State, b.ZipCode, 
	 b.Country,a.FrameCode, COUNT(OrderNumber) as NbrOrders, SUM(Pairs) as NbrPairs, 
	 case when fr.FrameDescription = fr.FrameNotes then 
		FrameDescription else  (FrameDescription + ' --  ' + FrameNotes) end Nomenclature
from
(
	select ClinicSiteCode, FrameCode, Pairs, po.OrderNumber,
		SUBSTRING(po.Demographic,5,2) as StatusCode, 
		SUBSTRING(po.Demographic,8,1) as PriorityCode,
		substring (po.Demographic,4,1) as bos,	
		REPLACE(SUBSTRING(po.Demographic, 1, 3), '*', '') AS MilRank
	from dbo.PatientOrder po left join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	where pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate) and
		pos.OrderStatusTypeID = '1'
		and ClinicSiteCode = @SiteCode
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1)= ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
) a		
left join 
(Select SiteCode, Address1, ADdress2, Address3, City, State, ZipCode, Country from dbo.SiteAddress 
	where AddressType = 'MAIL') b
on a.ClinicSiteCode =  b.SiteCode
inner join dbo.SiteCode sc on a.ClinicSiteCode = sc.SiteCode	
left join dbo.Frame fr on fr.FrameCode = a.FrameCode	
Group By ClinicSiteCode, SiteName, Address1, Address2, Address3, City, State, ZipCode,
Country, a.FrameCode, FrameDescription, FrameNotes
order by FrameCode


END
GO
/****** Object:  StoredProcedure [dbo].[GetClinicOrdersReportData]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
 Author:		Barb Fieldhausen
 Create date: 3 July 2013
 Description:	This stored procedure pulls all patient orders for a specific clinic, timeframe, priority and status.
		The resulting dataset populates the ClinicOrdersReport
 Change: 8 July 13 -- Added IsFOC for calculating number of Frames of Choice Selected
         9 July 13 -- Added MilRank for Reporting Purposes
	   24 July 13 -- Modified to reflect table changes from Order Status History to Patient Order Status, 
				Added function to calculate workdays 
		   5 Nov 13 -- Modified logic
		   2 Jul 15 -- baf - Added Clinic Dispense Column and Date Type
		   6 Aug 15 -- baf -- Add to only pull active orders
		  15 Sep 15 -- baf -- Added OrderStatus 19 - Lab Shipped To Patient
		  25 Apr 16 -- baf -- Added Status 16 to Clinic Checkin
		 01 Jan 18 - baf - Added Coatings
		 09 Feb 18 - jrp - Added block to handle all Status "0", fixed the addition of Lab Shipped to Patient on Lab Dispensed request, 
			and fixed audit to record same records as report.
-- ============================================= */
CREATE PROCEDURE [dbo].[GetClinicOrdersReportData]
@SiteCode	VARCHAR(6) = null,
@Status varchar(2) = null,
@Priority varchar(1) = null,
@FromDate datetime,
@ToDate datetime,
@ModifiedBy	VarChar(200),
@Type VarChar(20) = null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StatusType INT,
		@StatusType2 INT = NULL,
		@Order VarChar(16)
	
	SELECT @StatusType = CASE @Type WHEN 'Ordered' THEN 1
		WHEN 'Lab Received' THEN 2
		WHEN 'Lab Dispensed' THEN 7
		WHEN 'Received' THEN 11
		WHEN 'ClinicDispense' THEN 8
		WHEN 'ShipToPatient' THEN 19
		ELSE 0
	END 

	SELECT @StatusType2 = Case @StatusType WHEN 11 then 16 when 7 then 19 END		

	--Create Table #TmpOrderNbr (OrderNbr VarChar(16), OrderStatusTypeID int)
	Create Table #TmpOrderNbr (OrderNbr VarChar(16), OrderStatusTypeID int, OrderDate Date, LabRecDate Date,
		GEyesDate Date, LabDispense Date, ClinicDispense Date, ClinicRecFrLab Date, LabShipped Date)
	
	If @StatusType = 1 -- Ordered
		Begin
			Insert #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
			Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
			where OrderStatusTypeID = 1 and LEFT(OrderNumber,6) = @SiteCode
				and DateLastModified between @FromDate and @ToDate
		End
	If @StatusType = 2 -- Lab Received
		Begin
			Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
			Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
			where OrderStatusTypeID = 2 and LEFT(OrderNumber,6) = @SiteCode
				and DateLastModified between @FromDate and @ToDate
		End
	IF @StatusType = 7 -- Lab Dispensed
		If @StatusType2 = Null
			Begin
				Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
				Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
				where OrderStatusTypeID = 7 and LEFT(OrderNumber,6) = @SiteCode
					and DateLastModified between @FromDate and @ToDate			
			End
		ELSE
			BEGIN
				Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)-- Lab Dispensed and Ship To Patient
				Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
				where OrderStatusTypeID in (7,19) and LEFT(OrderNumber,6) = @SiteCode
					and DateLastModified between @FromDate and @ToDate			
			END
	IF @StatusType = 8 -- Clinic Dispense
		Begin
			Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
			Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
			where OrderStatusTypeID = 8 and LEFT(OrderNumber,6) = @SiteCode
				and DateLastModified between @FromDate and @ToDate
		END
	IF @StatusType = 11 -- Clinic Received
		BEGIN
			Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
			Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
			where OrderStatusTypeID in (11,16) and LEFT(OrderNumber,6) = @SiteCode
				and DateLastModified between @FromDate and @ToDate
		END
	IF @StatusType = 19 -- Lab Ship To Patient
		Begin
			Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
			Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
			where OrderStatusTypeID = 19 and LEFT(OrderNumber,6) = @SiteCode
				and DateLastModified between @FromDate and @ToDate
		END
	IF @StatusType = 0 -- All	
		Begin
			Insert into #TmpOrderNbr (OrderNbr, OrderStatusTypeID)
			Select OrderNumber, OrderStatusTypeID from dbo.PatientOrderStatus 
			where LEFT(OrderNumber,6) = @SiteCode
				and DateLastModified between @FromDate and @ToDate
		END
		
		Declare Stat Cursor
		For Select OrderNbr from #TmpOrderNbr
		
		Open Stat
		Fetch Stat into @Order
		While (@@FETCH_STATUS = 0)
			Begin
				Update #TmpOrderNbr set OrderDate = (Select CONVERT(VarChar(10),Max(DateLastModified),101)
					From dbo.PatientOrderStatus where OrderStatusTypeID = 1 and OrderNumber = @Order)
				where #TmpOrderNbr.OrderNbr = @Order
					
				Update #TmpOrderNbr set LabRecDate	= (Select CONVERT(VarChar(10),Max(DateLastModified),101)
					From dbo.PatientOrderStatus where OrderStatusTypeID = 2 and OrderNumber = @Order)
				where #TmpOrderNbr.OrderNbr = @Order
					
				If @Order like '009900%' 
					Begin
						Update #TmpOrderNbr set GEyesDate	= (Select CONVERT(VarChar(10),Max(DateLastModified),101)
							From dbo.PatientOrderStatus where OrderStatusTypeID = 6 and OrderNumber = @Order)
						where #TmpOrderNbr.OrderNbr = @Order
					End	
				
				Update #TmpOrderNbr set LabDispense	= (Select CONVERT(VarChar(10),Max(DateLastModified),101)
					From dbo.PatientOrderStatus where OrderStatusTypeID in (7,19) and OrderNumber = @Order)
				where #TmpOrderNbr.OrderNbr = @Order
				
				Update #TmpOrderNbr set ClinicDispense	= (Select CONVERT(VarChar(10),Max(DateLastModified),101)
					From dbo.PatientOrderStatus where OrderStatusTypeID = 8 and OrderNumber = @Order)	
				where #TmpOrderNbr.OrderNbr = @Order
					
				Update #TmpOrderNbr set ClinicRecFrLab	= (Select CONVERT(VarChar(10),Max(DateLastModified),101)
					From dbo.PatientOrderStatus where OrderStatusTypeID in (11,16) and OrderNumber = @Order)		
				where #TmpOrderNbr.OrderNbr = @Order
					
				Update #TmpOrderNbr set LabShipped	= (Select CONVERT(VarChar(10),Max(DateLastModified),101)
					From dbo.PatientOrderStatus where OrderStatusTypeID = 19 and OrderNumber = @Order)
				where #TmpOrderNbr.OrderNbr = @Order
					
				Fetch Stat into @Order		
			END
			Close Stat
			Deallocate Stat
			
Select  DISTINCT a.*, COUNT(DISTINCT a.OrderNumber) AS TotalClinicOrders, 
	tmp.OrderDate, tmp.LabRecDate, tmp.GEyesDate, tmp.LabDispense,tmp.ClinicDispense,
	tmp.ClinicRecFrLab, tmp.LabShipped,	CASE WHEN tmp.LabShipped IS NULL THEN 
		(Select dbo.fnWrkDays (tmp.OrderDate, tmp.ClinicRecFrLab))
	else (SELECT dbo.fnWrkDays(tmp.OrderDate, tmp.LabShipped)) end WrkDays
	from
	#tmpOrderNbr tmp inner join
	(
		select 	distinct
		po.ClinicSiteCode, po.LabSiteCode, po.Ordernumber, po.FrameCode,
		--case when fc.FrameNotes like '%FOC%' then 1 else 0 end isFOC, 
		Case when fc.IsFOC = 1 then 1 else 0 end isFOC,
		po.LensType, po.Tint, po.Coatings, po.Pairs, 
		SUBSTRING(po.Demographic,5,2) as StatusCode, 
		SUBSTRING(po.Demographic,8,1) as PriorityCode,
		substring (po.Demographic,4,1) as bos,	
		SUBSTRING (po.Demographic,1,3) as MilRank,
		ind.LastName, ind.FirstName, substring(MiddleName,1,1)as MI, 
		SiteName, RIGHT(inIDNbr.IDNumber,4)as LastFour
	from dbo.PatientOrder po 
		inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
		--inner join dbo.IndividualIDNumber inIDNbr on po.IndividualID_Patient = inIDNbr.IndividualID
        cross apply (select Top 1 * from dbo.IndividualIDNumber where IDNumberType in ('SSN','FSN','DIN') and IsActive=1 and po.IndividualID_Patient=IndividualID order by IDNumberType ) inIDNbr
		inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
		left join
		(Select OrderNbr from #TmpOrderNbr) ton on po.OrderNumber = ton.OrderNbr
		inner join dbo.Frame fc on po.FrameCode = fc.FrameCode
	where SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		--AND pos.IsActive = 1
		and po.IsActive = 1 and po.ModifiedBy <> 'SSIS' ) a on tmp.OrderNbr = a.OrderNumber
Group by ClinicSiteCode, LabSiteCode, a.OrderNumber, FrameCode, isFOC, LensType, Tint,
	Coatings, Pairs, StatusCode, PriorityCode, bos , MilRank, LastName, FirstName, MI, SiteName,
	LastFour, tmp.OrderDate, tmp.LabRecDate, tmp.LabDispense, tmp.GEyesDate, tmp.clinicdispense,
	tmp.clinicRecFrLab, tmp.LabShipped
	
	
	------------------  Audit MultiRead 10/21/14 -- BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'PatientIDs: ') + CAST(po.IndividualID_Patient as VarChar(20)) + ', '
	from dbo.PatientOrder po 
		--inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
		----inner join dbo.IndividualIDNumber inIDNbr on po.IndividualID_Patient = inIDNbr.IndividualID
		--cross apply (select Top 1 * from dbo.IndividualIDNumber where IDNumberType in ('SSN','FSN','DIN') and IsActive = 1 and po.IndividualID_Patient=IndividualID order by IDNumberType ) inIDNbr
		--inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
        Inner join
		(Select OrderNbr from #TmpOrderNbr) ton on po.OrderNumber = ton.OrderNbr
		--inner join dbo.Frame fc on po.FrameCode = fc.FrameCode
	--where SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
	--	and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
	--	and (@SiteCode is null or po.ClinicSiteCode = @SiteCode) 
		
	Select @Notes = Coalesce(@Notes, 'Clinic Orders Report Data - OrderNumbers: ') + po.OrderNumber + ', '
	from dbo.PatientOrder po 
		--inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
		--cross apply (select Top 1 * from dbo.IndividualIDNumber where IDNumberType in ('SSN','FSN','DIN') and IsActive = 1 and po.IndividualID_Patient=IndividualID order by IDNumberType ) inIDNbr
		--inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
		Inner join
		(Select OrderNbr from #TmpOrderNbr) ton on po.OrderNumber = ton.OrderNbr
	--	inner join dbo.Frame fc on po.FrameCode = fc.FrameCode
	--where (@Status is Null or @Status = SUBSTRING(po.Demographic,5,2))
	--	and (@Priority is Null or @Priority = SUBSTRING(po.Demographic,8,1))
	--	and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
		
	If @PatientList is not null 
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	
	---------------------------------------------------	
		
Delete from #TmpOrderNbr
Drop Table #TmpOrderNbr

END

--Select  DISTINCT a.*, COUNT(DISTINCT a.OrderNumber) AS TotalClinicOrders, 
--	b.OrderDate, c.LabRecDate, d.GEyesDate, e.LabDispense,f.ClinicDispense,g.ClinicRecFrLab,
--	h.LabShipped,
--	CASE WHEN h.LabShipped IS NULL THEN 
--		(Select dbo.fnWrkDays (b.OrderDate, g.ClinicRecFrLab))
--	else (SELECT dbo.fnWrkDays(b.OrderDate, h.LabShipped)) end WrkDays
--	from
--	(
--		select 	distinct
--		po.ClinicSiteCode, po.LabSiteCode, po.Ordernumber, po.FrameCode,
--		--case when fc.FrameNotes like '%FOC%' then 1 else 0 end isFOC, 
--		Case when fc.IsFOC = 1 then 1 else 0 end isFOC,
--		po.LensType, po.Tint, po.Coatings, po.Pairs, 
--		SUBSTRING(po.Demographic,5,2) as StatusCode, 
--		SUBSTRING(po.Demographic,8,1) as PriorityCode,
--		substring (po.Demographic,4,1) as bos,	
--		SUBSTRING (po.Demographic,1,3) as MilRank,
--		ind.LastName, ind.FirstName, substring(MiddleName,1,1)as MI, 
--		SiteName, RIGHT(inIDNbr.IDNumber,4)as LastFour
--	from dbo.PatientOrder po 
--		inner join dbo.Individual ind on po.IndividualID_Patient = ind.ID
--		--inner join dbo.IndividualIDNumber inIDNbr on po.IndividualID_Patient = inIDNbr.IndividualID
--        cross apply (select Top 1 * from dbo.IndividualIDNumber where IDNumberType in ('SSN','FSN','DIN') and IsActive=1 and po.IndividualID_Patient=IndividualID order by IDNumberType ) inIDNbr
--		inner join dbo.SiteCode sc on sc.SiteCode = po.LabSiteCode
--		left join
--		(Select OrderNbr from #TmpOrderNbr) ton on po.OrderNumber = ton.OrderNbr
--		inner join dbo.Frame fc on po.FrameCode = fc.FrameCode
--	where SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
--		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
--		and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
--		--AND pos.IsActive = 1
--		and po.IsActive = 1 and po.ModifiedBy <> 'SSIS'
----	GROUP BY --pos.OrderStatusTypeID, 
----		po.ClinicSiteCode, po.LabSiteCode, po.OrderNumber, po.FrameCode,
----		fc.FrameNotes, po.LensType, po.Tint, po.Coatings, po.Pairs, po.Demographic, ind.LastName, ind.FirstName,
----		ind.MiddleName, sc.SiteCode, sc.SiteName, inIDNbr.IDNumber, fc.IsFOC
--	--HAVING (@StatusType IS NULL OR pos.OrderStatusTypeID = @StatusType) OR
--	--	(@StatusType2 is null or pos.OrderStatusTypeID = @StatusType2)
--) a
--left join
--(select OrderNumber, CONVERT(VarChar(10),Max(DateLastModified),101) as OrderDate 
--	from dbo.PatientOrderStatus 
--	where OrderStatusTypeID = 1 and ModifiedBy <> 'SSIS' --and IsActive = 1
--		and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	Group by OrderNumber
--) b on a.OrderNumber = b.OrderNumber
--left join 
--(select OrderNumber, CONVERT(VarChar(10),Max(DateLastModified),101) as LabRecDate 
--	from dbo.PatientOrderStatus 
--	where OrderStatusTypeID = 2  and ModifiedBy <> 'SSIS'--and IsActive = 1
--			and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	Group by ORderNumber
--) c on a.OrderNumber = c.OrderNumber
--left join
--(select OrderNumber, CONVERT(VarChar(10),Max(DateLastModified),101) as GEyesDate
--	from dbo.PatientOrderStatus 
--	where OrderStatusTypeID = 6 and ModifiedBy <> 'SSIS'--and IsActive = 1
--			and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	Group by ORderNumber
--) d on a.OrderNumber = d.OrderNumber
--left join
--(select OrderNumber, CONVERT(VarChar(10),Max(DateLastModified),101) as LabDispense 
--	from dbo.PatientOrderStatus 
--	where OrderStatusTypeID IN(7,19) --and IsActive = 1
--		and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	Group by ORderNumber
--)e on a.OrderNumber = e.OrderNumber
--left join
--(select OrderNumber, CONVERT(VarChar(10),Max(DateLastModified),101) as ClinicDispense
--	from dbo.PatientOrderStatus 
--	where OrderStatusTypeID = 8 --and IsActive = 1
--		and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	Group by ORderNumber
--) f on a.OrderNumber = f.OrderNumber
--left join
--(select OrderNumber, CONVERT(VarChar(10),Max(DateLastModified),101) as ClinicRecFrLab 
--	from dbo.PatientOrderStatus 
--	where OrderStatusTypeID IN (11,16) --and IsActive = 1 11 - Clinic Checked in, 16 - Clinic Check in - No Lab Checkout
--		and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	Group by ORderNumber
--) g on a.OrderNumber = g.OrderNumber
--LEFT JOIN
--(SELECT OrderNumber, CONVERT(VARCHAR(10),MAX(DateLastMOdified),101) AS LabShipped
--	FROM dbo.PatientOrderStatus
--	WHERE OrderStatusTypeID = 19 -- Lab shipped to Patient
--		and OrderNumber in (Select OrderNbr from #TmpOrderNbr)
--	GROUP BY OrderNumber
--) h ON a.OrderNumber = h.OrderNumber

--where OrderDate is not NULL or GEyesDate is not null
--GROUP BY a.ClinicSiteCode, a.LabSiteCode,a.OrderNumber,a.FrameCode, a.isFOC,a.LensType, a.Tint,
--	a.Coatings,a.Pairs,a.StatusCode,a.PriorityCode,a.bos,a.MilRank, a.LastName, a.FirstName, a.MI,
--	a.SiteName, a.LastFour,b.OrderDate, g.ClinicRecFrLab, c.LabRecDate, e.LabDispense,
--	f.ClinicDispense, d.GEyesDate,h.LabShipped 
--order by OrderNumber, LastName

--Delete from #TmpOrderNbr
--Drop table #TmpOrderNbr

--End
GO
/****** Object:  StoredProcedure [dbo].[GetClinicDispenseData]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9 July 2013
-- Description:	This stored procedure pulls all patient orders for a specific clinic, timeframe, priority and status.
--		The resulting dataset populates the ClinicDispenseReport
-- Change:  25 Apr 16 -- baf - Added Status 16 - Clinic Check in - NO Lab Checkout 
--			01 Jan 18 - baf - Added Coatings
--                      07 Mar 18 - jrp - fixed for records with no Coatings and multiple IDs and records with statustype of 1 and 6 so you don't get multiple rows.
-- =============================================
CREATE PROCEDURE [dbo].[GetClinicDispenseData]

@FromDate datetime,
@ToDate datetime,
@SiteCode	VARCHAR(6) = null,
@Priority VarChar(1) = null,
@Status VarChar(2) = null,
@ModifiedBy		VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--------------- Audit Multi Read  10/21/14 - BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'PatientIDs: ') + Cast(po.IndividualID_Patient as VarChar(20)) + ', '
	from dbo.PatientOrder po right join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	left join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	left join dbo.IndividualIDNumber inIDNbr on ind.ID = inIDNbr.ID
	where pos.IsActive = 1 and pos.OrderStatusTypeID IN (11,16)
		and ClinicSiteCode = @SiteCode
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
		and inIDNbr.IsActive = 1

	Select @Notes = Coalesce(@Notes, 'Clinic Dispense Report OrderNumbers: ') + po.OrderNumber + ', '
		from dbo.PatientOrder po right join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	left join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	left join dbo.IndividualIDNumber inIDNbr on ind.ID = inIDNbr.ID
	where pos.IsActive = 1 and pos.OrderStatusTypeID = 11
		and ClinicSiteCode = @SiteCode
		and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1))
		and inIDNbr.IsActive = 1
		
	If @Notes is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	------------------------------------------------

	select distinct po.ordernumber, po.ClinicSiteCode, pos.LabSiteCode, po.framecode, po.pairs, po.tint, po.lenstype,
		po.Coatings, ind.LastName, ind.FirstName, SUBSTRING(ind.MiddleName,1,1) as MI, po.demographic,
		RIGHT(inIDNbr.IDNumber,4) IDNbr,SUBSTRING(po.Demographic,5,2) as StatusCode, 
		SUBSTRING(po.Demographic,8,1) as PriorityCode,
		substring (po.Demographic,4,1) as bos,	
		REPLACE(SUBSTRING(po.Demographic, 1, 3), '*', '') AS MilRank, po.ONBarCode,
		Cast(b.DateLastModified as Date) as DateOrdered, Cast(c.DateLastModified as Date) as LabReceivedOrder
	from dbo.PatientOrder po 
	right join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	left join dbo.Individual ind on ind.ID = po.IndividualID_Patient
	cross apply( select Top 1 * from dbo.IndividualIDNumber IndIdNum where ind.ID = IndIdNum.IndividualID and IndIdNum.IsActive = 1 order by IndIdNum.IDNumberType) inIDNbr
	cross apply(Select Top 1 DateLastModified, pos2.OrderStatusTypeID from dbo.PatientOrderStatus pos2 where po.OrderNumber = pos2.OrderNumber and pos2.OrderStatusTypeID in (1,6) order by pos2.OrderStatusTypeID desc ) b
    left join (Select Max(DateLastModified) as DateLastModified, OrderNumber from dbo.PatientOrderStatus pos3 where OrderStatusTypeID = 2 Group by OrderNumber) c on po.OrderNumber = c.OrderNumber	where pos.IsActive = 1 and pos.OrderStatusTypeID IN (11,16)
		 and pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)
         --c.DateLastModified is not null and b.DateLastModified is not null and
		 --b.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)  and
		 and po.ClinicSiteCode = @SiteCode 
		 and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
		 and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
order by LabSiteCode, LastName

END
GO
/****** Object:  StoredProcedure [dbo].[GetAllActiveOrdersBySiteCode]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Dial
-- Create date: 9/29/2015
-- Description:	This stored procedure retrieves all
--		active orders for a specific location (SiteCode)
-- =============================================
CREATE PROCEDURE [dbo].[GetAllActiveOrdersBySiteCode]
	@SiteCode VARCHAR(6),
	@ModifiedBy VARCHAR(200),
	@StartDate DATETIME,
	@EndDate DateTime
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @RecCnt INT,
		@LabSiteCode VarChar(6),
		@SiteType VARCHAR(6)
		
	IF LEFT(@SiteCode,1) NOT IN ('M','S')
		BEGIN
			SET @SiteType = 'CLINIC'
		End	
	ELSE
		BEGIN
			SET @SiteType = 'LAB'
		END
    
    CREATE TABLE #temp_ActiveOrders
    (OrderNumber VARCHAR(16), SiteType VARCHAR(6), StatusDate DATETIME,
		StatusComment VARCHAR(256), FrameCode VARCHAR(5), LensType VARCHAR(10), 
		LastName VARCHAR(25), FirstName VARCHAR(25), MiddleName VARCHAR(25),
		LastFour VARCHAR(4))
		
	IF @SiteType = 'Clinic'
		Begin
			INSERT INTO #temp_ActiveOrders
			SELECT pos.OrderNumber, CASE WHEN @SiteType = 'LAB' THEN po.ClinicSiteCode
				WHEN @SiteType = 'CLINIC' THEN pos.LabSiteCode END SiteCode, 
				pos.DateLastModified AS StatusDate, 
				CASE WHEN RTRIM(pos.StatusComment) = '' AND pos.OrderStatusTypeID = 2 THEN
					'Lab Received Order' ELSE StatusComment END StatusComment, po.FrameCode,
				po.LensType, ind.LastName, ind.FirstName, ind.MiddleName,
				RIGHT(nbr.IDNumber,4) AS LastFour
			FROM dbo.PatientOrder po
				INNER JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
				INNER JOIN dbo.Individual ind ON po.IndividualID_Patient = ind.ID
				INNER JOIN dbo.IndividualIDNumber nbr ON ind.id = nbr.IndividualID
 			WHERE pos.DateLastModified between @StartDate and @EndDate AND
 				pos.OrderStatusTypeID NOT IN (8,19,14,15,16) and
 				po.IsActive = 1 AND pos.IsActive = 1
				AND po.ClinicSiteCode = @SiteCode
				AND nbr.IDNumberType in ('SSN','FSN')
		End
	ELSE
		Begin
			INSERT INTO #temp_ActiveOrders
			SELECT pos.OrderNumber, CASE WHEN @SiteType = 'LAB' THEN po.ClinicSiteCode
				WHEN @SiteType = 'CLINIC' THEN pos.LabSiteCode END SiteCode, 
				pos.DateLastModified AS StatusDate, 
				CASE WHEN RTRIM(pos.StatusComment) = '' AND pos.OrderStatusTypeID = 2 THEN
					'Lab Received Order' ELSE StatusComment END StatusComment, po.FrameCode,			
				po.LensType, ind.LastName, ind.FirstName, ind.MiddleName,
				RIGHT(nbr.IDNumber,4) AS LastFour
			FROM dbo.PatientOrder po
				INNER JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
				INNER JOIN dbo.Individual ind ON po.IndividualID_Patient = ind.ID
				INNER JOIN dbo.IndividualIDNumber nbr ON ind.id = nbr.IndividualID
 			WHERE pos.DateLastModified between @StartDate and @EndDate and
 				po.IsActive = 1 AND pos.IsActive = 1 and pos.LabSiteCode = @SiteCode
 				and pos.OrderStatusTypeID not in (8,14,15,16,19)
 				and nbr.IDNumberType  in ('SSN','FSN')
		End
		
	SELECT @RecCnt = COUNT(*) FROM #temp_ActiveOrders

	-------------------- audit multi read -------------------------------------------
	Declare @ReadDate datetime = GetDate()
	Declare @Notes VarChar(Max)
	Declare @PatientIDList VarChar(Max)
	Declare @Site VarChar(10)
	
	 If @SiteCode is NULL
			Set @Site = 'ALL SITES'
	 ELSE
			Set @Site = @SiteCode;
	
	If @RecCnt > 100 
		Set @PatientIDList = 'Active Order List Is Greater than 100 records for Site: ' + @Site +
			'Record Cnt = ' + CAST(@RecCnt as VarChar(20)) + '   For Time Frame: ' + CAST(@StartDate AS VARCHAR(35)) + ' TO '
			+ CAST(@EndDate AS VarChar(35))
	ELSE
	Select DISTINCT @PatientIDList =  Coalesce(@PatientIDList,'OrderNumber: ') + OrderNumber + ', '
	FROM #temp_ActiveOrders
	
	Set @Notes = 'Active OrderNumbers for SiteCode = ' + @Site
		+ ', Time Frame: ' + CAST(@StartDate AS VARCHAR(35)) + ' TO '+ CAST(@EndDate AS VarChar(35))	
	If @Notes is not null and @PatientIDList is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientIDList, @Notes;
	---------------	
	
	SELECT * FROM #temp_ActiveOrders ORDER BY OrderNumber

DROP TABLE #temp_ActiveOrders;
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetRpt54Data]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		Barb Fieldhausen
 Create date: 9/23/2013
 Description:	This stored procedure retrieves the data necessary
		to complete the Report54 -- Summary Report for All Orders
		Processed for period specified.  SubReports contain the bulk
		of the data for the report.  Below is the data to complete
		the Single and Multi-Vision Lens data.
 Modified: 3/26/15 - baf - Per SFC Lopez, removed (Pairs * 2) from subquery
		Was calculating by lenses, want it by pairs, add carry over orders, 
		Orders recieved but not processed, as well as Total orders received.
 ============================================= */
CREATE PROCEDURE [dbo].[GetRpt54Data]

	@FromDate DateTime,
	@ToDate DateTime,
	@LabSiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

select b.LabSiteCode,  TotalOrders, TotalProcessed, TotalRejects, CarryOver, SUM(SNAD) as TSNAD, SUM(SNRet) as TSNRET, 
	SUM(SMAD) as TSMAD, SUM(SMRet) as TSMRet, SUM(SAAD) as TSAAD, SUM(SARet) as TSARet,
	SUM(SFAD) as TSFAD, sum(SFRet) as TSFRet, SUM(SPDRL) as TSPDRL, SUM(SFmrPOW) as TSPOW,
	SUM(SCAD) as TSCAD, SUM(SPAD) as TSPAD, SUM(SBAD) as TSBAD, SUM(SFRes) as TSFRES, 
	SUM(SARes) as TSARes, SUM(SANG) as TSANG, sum(SFNG) as TSFNG,SUM(SNRes) as TSNRes,
	SUM(SNDepAD) as TSNDep, SUM(SMRes) as TSMRes, SUM(SDodRemCONUS) as TSDRCONUS, 
	SUM(SVABen) as TSVABen, SUM(SIMET) as TSIMET, SUM(Sother) as TSOther,SUM(MNAD) as TMNAD, 
	SUM(MNRet) as TMNRET, SUM(MMAD) as TMMAD, SUM(MMRet) as TMMRet, SUM(MAAD) as TMAAD, 
	SUM(MARet) as TMARet, SUM(MFAD) as TMFAD, sum(MFRet) as TMFRet, SUM(MPDRL) as TMPDRL, 
	SUM(MFmrPOW) as TMPOW, SUM(MCAD) as TMCAD, SUM(MPAD) as TMPAD, SUM(MBAD) as TMBAD, 
	SUM(MFRes) as TMFRES, SUM(MARes) as TMARes, SUM(MANG) as TMANG, sum(MFNG) as TMFNG,
	SUM(MNRes) as TMNRes, SUM(MNDepAD) as TMNDep, SUM(MMRes) as TMMRes, 
	SUM(MDodRemCONUS) as TMDRCONUS, sUM(MVABen) as TMVABen, SUM(MIMET) as TMIMET, SUM(Mother) as TMOther
from
(
	select LabSiteCode, 
		case when a.bos = 'N' and statuscode in (11,14) and IsMultiVision = 0 then Sum(Pairs) else 0 end SNAD,
		case when a.bos = 'N' and statuscode = 31  and IsMultivision = 0 then Sum(Pairs) else 0 end SNRet,
		case when a.bos = 'M' and statuscode = 11 and IsMultiVision = 0 then Sum(Pairs) else 0 end SMAD,
		case when a.bos = 'M' and statuscode = 31 and IsMultiVision = 0 then Sum(Pairs) else 0 end SMRet,
		case when a.bos = 'A' and statuscode in (11,14) and IsMultiVision = 0 then Sum(Pairs) else 0 end SAAD,
		case when a.bos = 'A' and statuscode = 31 and IsMultiVision = 0 then Sum(Pairs) else 0 end SARet,
		case when a.bos = 'F' and statuscode in (11,14) and IsMultiVision = 0 then Sum(Pairs) else 0 end SFAD,
		case when a.bos = 'F' and statuscode = 31 and IsMultiVision = 0 then Sum(Pairs) else 0 end SFRet,
		case when statuscode = 32  and IsMultiVision = 0 then Sum(Pairs) else 0 end SPDRL,
		case when statuscode = 36  and IsMultiVision = 0 then Sum(Pairs) else 0 end SFmrPOW,
		case when a.bos = 'C' and statuscode = 11 and IsMultiVision = 0 then Sum(Pairs) else 0 end SCAD,
		case when a.bos = 'P' and statuscode = 11 and IsMultiVision = 0 then Sum(Pairs) else 0 end SPAD,
		case when a.bos = 'B' and statuscode = 11 and IsMultiVision = 0 then Sum(Pairs) else 0 end SBAD,
		case when a.bos = 'F' and statuscode = 12 and IsMultiVision = 0 then Sum(Pairs) else 0 end SFRes,
		case when a.bos = 'A' and statuscode = 12 and IsMultiVision = 0 then Sum(Pairs) else 0 end SARes,
		case when a.bos = 'A' and statuscode = 15 and IsMultiVision = 0 then Sum(Pairs) else 0 end SANG,
		case when a.bos = 'F' and statuscode = 15 and IsMultiVision = 0 then Sum(Pairs) else 0 end SFNG,
		case when a.bos = 'N' and statuscode = 12 and IsMultiVision = 0 then Sum(Pairs) else 0 end SNRes,
		case when a.bos = 'N' and statuscode = 41 and IsMultiVision = 0 then Sum(Pairs) else 0 end SNDepAD,
		case when a.bos = 'M' and statuscode = 12 and IsMultiVision = 0 then Sum(Pairs) else 0 end SMRes,
		case when a.bos = 'K' and statuscode = 55 and IsMultiVision = 0 then Sum(Pairs) else 0 end SDodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 and IsMultiVision = 0 then Sum(Pairs) else 0 end SVABen,
		case when a.bos = 'K' and statuscode = 71 and IsMultiVision = 0 then Sum(Pairs) else 0 end SIMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) and IsMultivision = 0 then Sum(Pairs) else 0 end SOther,
		case when a.bos = 'N' and statuscode in (11,14) and IsMultiVision = 1 then Sum(Pairs) else 0 end MNAD,
		case when a.bos = 'N' and statuscode = 31  and IsMultivision = 1 then Sum(Pairs) else 0 end MNRet,
		case when a.bos = 'M' and statuscode = 11 and IsMultiVision = 1 then Sum(Pairs) else 0 end MMAD,
		case when a.bos = 'M' and statuscode = 31 and IsMultiVision = 1 then Sum(Pairs) else 0 end MMRet,
		case when a.bos = 'A' and statuscode in (11,14) and IsMultiVision = 1 then Sum(Pairs) else 0 end MAAD,
		case when a.bos = 'A' and statuscode = 31 and IsMultiVision = 1 then Sum(Pairs) else 0 end MARet,
		case when a.bos = 'F' and statuscode in (11,14) and IsMultiVision = 1 then Sum(Pairs) else 0 end MFAD,
		case when a.bos = 'F' and statuscode = 31 and IsMultiVision = 1 then Sum(Pairs) else 0 end MFRet,
		case when statuscode = 32  and IsMultiVision = 1 then Sum(Pairs) else 0 end MPDRL,
		case when statuscode = 36  and IsMultiVision = 1 then Sum(Pairs) else 0 end MFmrPOW,
		case when a.bos = 'C' and statuscode = 11 and IsMultiVision = 1 then Sum(Pairs) else 0 end MCAD,
		case when a.bos = 'P' and statuscode = 11 and IsMultiVision = 1 then Sum(Pairs) else 0 end MPAD,
		case when a.bos = 'B' and statuscode = 11 and IsMultiVision = 1 then Sum(Pairs) else 0 end MBAD,
		case when a.bos = 'F' and statuscode = 12 and IsMultiVision = 1 then Sum(Pairs) else 0 end MFRes,
		case when a.bos = 'A' and statuscode = 12 and IsMultiVision = 1 then Sum(Pairs) else 0 end MARes,
		case when a.bos = 'A' and statuscode = 15 and IsMultiVision = 1 then Sum(Pairs) else 0 end MANG,
		case when a.bos = 'F' and statuscode = 15 and IsMultiVision = 1 then Sum(Pairs) else 0 end MFNG,
		case when a.bos = 'N' and statuscode = 12 and IsMultiVision = 1 then Sum(Pairs) else 0 end MNRes,
		case when a.bos = 'N' and statuscode = 41 and IsMultiVision = 1 then Sum(Pairs) else 0 end MNDepAD,
		case when a.bos = 'M' and statuscode = 12 and IsMultiVision = 1 then Sum(Pairs) else 0 end MMRes,
		case when a.bos = 'K' and statuscode = 55 and IsMultiVision = 1 then Sum(Pairs) else 0 end MDodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 and IsMultiVision = 1 then Sum(Pairs) else 0 end MVABen,
		case when a.bos = 'K' and statuscode = 71 and IsMultiVision = 1 then Sum(Pairs) else 0 end MIMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) and IsMultivision = 1 then Sum(Pairs) else 0 end MOther
	from
	(
		select pos.OrderStatusTypeID, po.ClinicSiteCode, po.LabSiteCode,
			REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(i.Demographic, 5,2) AS statuscode,
			SUBSTRING(i.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(i.Demographic, 4, 1) AS bos,  po.LensType,
			po.Pairs, f.FrameDescription, f.FrameNotes,
			IsMultivision
		from dbo.PatientOrder po left join dbo.PatientOrderStatus pos
		on po.OrderNumber = pos.OrderNumber
		inner join Individual i on i.ID = po.IndividualID_Patient
		inner join Frame f on f.FrameCode = po.FrameCode
		where pos.DateLastModified between @FromDate and @ToDate and 
		pos.OrderStatusTypeID IN (7,19) and pos.LabSiteCode = @LabSiteCode
	) a
	Group by LabSiteCode, bos, statuscode, IsMultivision
)b
Left join (Select LabSiteCode, SUM(OrdersReceived) as TotalOrders,
	SUM(Dispensed) as TotalProcessed, Sum(Rejected) as TotalRejects,
	(Sum(OrdersReceived) - SUM(Dispensed) - SUM(Rejected)) as CarryOver
From
(
	Select LabSiteCode,COUNT(OrderNumber) as OrdersReceived, 
		Case when OrderStatusTypeID IN (7,19) then	
			COUNT(OrderNumber)  else 0 end Dispensed,
		Case when OrderStatusTypeID = 3 then
			Count(OrderNumber) else 0 end Rejected
	From (		
		Select *  from dbo.PatientOrderStatus
		Where DateLastModified between @FromDate and @ToDate and 
			 LabSiteCode = @LabSiteCode and IsActive = 1
		) a1	
	Group by OrderStatusTypeID, LabSiteCode
) b1 Group by LabSiteCode )c
on c.LabSiteCode = b.LabSiteCode
Group by b.LabSiteCode, c.TotalOrders, c.TotalProcessed, c.CarryOver, c.TotalRejects

END
GO
/****** Object:  StoredProcedure [dbo].[GetReprintCountList]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 March 2017
-- Description:	Given a sitecode and orderstatustypeid, a list
--	of possible items for reprint are returned with the count,
--	modifiedBy and OrderKey (datetime of the record).
-- =============================================
CREATE PROCEDURE [dbo].[GetReprintCountList]
	-- Add the parameters for the stored procedure here
	@OrderStatusTypeID VarChar(5),
	@SiteCode VarChar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate DateTime = DateAdd(dd,-7,GetDate()),
	@EndDate DateTime = GetDate(),
	@LenStatus int, @Status1 int = 0, @Status2 int = 0
	
	Set @LenStatus = LEN(@OrderStatusTypeID)
	IF @LenStatus <= 2
		Begin
			Set @Status1 = @OrderSTatusTypeID
		End
	Else if @LenStatus = 5
		Begin
			Set @Status1 = Cast(Left(@OrderStatustypeID,2) as int)
			Set @Status2 = Cast(Right(@OrderstatusTypeID,2) as int)
		End
	ELSE if @LenStatus = 4
		Begin
			Set @Status1 = Cast(Left(@OrderStatustypeID,1) as int)
			Set @Status2 = Cast(Right(@OrderstatusTypeID,2) as int)
		End
	
	IF @ORderStatusTypeID in ('11,16','11','16','8')  -- Clinic Labels
		Begin	
			Select  SUM(RecCnt) as RecCount, ModifiedBy, OrderKey from
			(
			SELECT Count(pos.OrderNumber) as RecCnt, pos.OrderNumber, b.ModifiedBy,  CAST(b.DateLastModified AS SMALLDATETIME) AS OrderKey,
				ShipToPatient, ClinicShipToPatient
			FROM dbo.PatientOrderStatus pos inner join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
			Right Join
			(SElect ORderNumber, DateLastModified, ModifiedBy from dbo.PatientOrderStatus where
			 CAST(DateLastModified AS DATETIME) between @StartDate and @EndDate and
			 LEFT(OrderNumber,6) = @SiteCode and OrderStatusTypeID in (@Status1, @Status2)--@OrderStatusTypeID
			) b on pos.OrderNumber = b.OrderNumber
			WHERE left(pos.OrderNumber,6) =  @SiteCode  
				AND po.ShipToPatient = 1 or po.ClinicShipToPatient = 1
				and pos.OrderStatusTypeID in (@Status1, @Status2)
			Group by b.ModifiedBy, pos.DateLastModified, pos.OrderNumber, b.DateLastModified, ShipToPatient, ClinicShipToPatient
			) a
			where a.ClinicShipToPatient = 1 or a.ShipToPatient = 1
			Group by ORderKey, ModifiedBy
		END
		ELSE IF @OrderStatusTypeID = '2'  -- Lab 771's
			BEGIN
				Select COUNT(OrderNumber) as RecCount, ModifiedBy, OrderKey
				From 
				(
				Select pos.OrderNumber, pos.ModifiedBy,  CAST(pos.DateLastModified AS SMALLDATETIME) AS OrderKey from dbo.PatientOrderStatus pos
					Inner Join dbo.PatientOrder po on pos.OrderNumber = po.OrderNumber
					inner join dbo.SiteCode sc on pos.LabSiteCode = sc.SiteCode 
				where pos.LabSiteCode = @SiteCode --and ShipToPatientLab = 1 and
					and OrderStatusTypeID = 2 and pos.DateLastModified between @StartDate and @EndDate 
				)a
				Group by ModifiedBy, OrderKey
				Order by ModifiedBy, OrderKey	
			END
		ELSE IF @OrderStatusTypeID in ('7,19','7','19') -- Lab Labels
			BEGIN
				Select COUNT(*) as RecCount, ModifiedBy, OrderKey from
				(
				SELECT pos.OrderNumber, pos.ModifiedBy,  CAST(pos.DateLastModified AS SMALLDATETIME) AS OrderKey
				FROM dbo.PatientOrderStatus pos inner join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
				WHERE po.ShiptoPatient = 1 and 
					pos.LabSiteCode =  @SiteCode and (OrderStatusTypeID in (@Status1, @Status2))
					AND pos.DateLastModified between @StartDate and @EndDate
				) a
				Group by OrderKey, ModifiedBy
				Order BY OrderKey desc	
			END
END
GO
/****** Object:  StoredProcedure [dbo].[GetReprint771]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  3/26/2015 - baf - Reprint DDForm 771s does not require
--		Specific Order Status OR SiteCode
--		2/23/2017 - baf - Added DispenseComments
--		1/5/18 - baf - Added Coatings
--		9/6/2018 - baf - changed user comment 1 to add
--			comment from lab redirection to usercomment1
-- =============================================
*/
CREATE PROCEDURE [dbo].[GetReprint771] 
@SiteCode	VARCHAR(6) = Null,
@OrderNbr Varchar(16) = Null,
@ModifiedBy	VarChar(200)

AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	---------- Audit Multi Read - 10/21/14 BAF ---------------
	Declare @ReadDate DateTime = GetDate()
	Declare @PatientList VarChar(Max)
	Declare @Notes VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'PatientIDs: ') + CAST(a.IndividualID_Patient as VarChar(20)) + ', '
	FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	WHERE a.IsActive = 1
	AND b.IsActive = 1
	AND k.IsActive = 1
	--AND b.OrderStatusTypeID in (1,6,4,9,10)
	--AND (b.OrderStatusTypeID = 1 OR b.OrderStatusTypeID = 6 OR b.OrderStatusTypeID = 4 
	--OR b.OrderStatusTypeID = 9 OR b.OrderStatusTypeID = 10)
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	--AND (@SiteCode is Null or b.LabSiteCode = @SiteCode)
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
	
	Select @Notes = Coalesce(@Notes, 'Reprint 771 Orders: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualIDNumber k ON a.IndividualID_Patient = k.IndividualID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	WHERE a.IsActive = 1
	AND b.IsActive = 1
	AND k.IsActive = 1
	--AND b.OrderStatusTypeID in (1,6,4,9,10)
	--AND (b.OrderStatusTypeID = 1 OR b.OrderStatusTypeID = 6 OR b.OrderStatusTypeID = 4 
	--OR b.OrderStatusTypeID = 9 OR b.OrderStatusTypeID = 10)
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	--AND (@SiteCode is Null or b.LabSiteCode = @SiteCode)
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
	
	If @Notes is not null 
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
	------------------------------------------------------------
	
	Declare @IDCnt int = 1, @IDNbr VarChar(10), @PatID int
	
	Select @PatID = IndividualID_Patient from dbo.PatientOrder
	where OrderNumber = @OrderNbr
	
	Select @IDCnt = COUNT(IDNumber) from dbo.IndividualIDNumber 
	where IndividualID = @PatID
	
	IF @IDCnt = 1
		Begin
			Select @IDNbr = IDNumber from dbo.IndividualIDNumber
			where IndividualID = @PatID --and IDNumberType = 'SSN'
		End
	ELSE
		Begin
			Select @IDNbr = IDNumber from dbo.IndividualIDNumber
			where IndividualID = @PatID and IDNumberType = 'DIN' and IsActive = 1
		End

	Create Table #OrderPos (RowID int, OrderNumber VarChar(16), OrderStatus Int, Comment VarChar(300),
		DateModified DateTime)
	Insert into #OrderPos
	SELECT ROW_NUMBER() OVER(ORDER BY DateLastModified DESC) AS Row#,		OrderNumber, OrderStatusTypeID, StatusComment, DateLastModified 	FROM dbo.PatientOrderStatus pos	WHERE pos.OrderNumber = @OrderNbr
	
	Declare @Comment VarChar(300) = Null, @RowID int
	Select @RowID = RowID from #OrderPos
	where OrderStatus = 4 -- and RowID < 2

	If @RowID <= 2
		Begin
			Select @Comment = Comment from #OrderPOS where RowID = @RowID
		End
	ELSE
		Begin
			Set @Comment = ''
		ENd


	SELECT distinct a.OrderNumber,
		SUBSTRING(a.Demographic, 7, 1) AS sexcode,
		SUBSTRING(a.Demographic, 1, 3) AS rankcode,
		SUBSTRING(a.Demographic, 5,2) AS statuscode,
		SUBSTRING(a.Demographic, 8, 1) AS orderpriority,
		SUBSTRING(a.Demographic, 4, 1) AS bos,
		SUBSTRING(m.FirstName, 1,1) + SUBSTRING(m.MiddleName, 1,1) + SUBSTRING(m.LastName,1,1) AS techinitials,
		a.LensType,
		a.LensMaterial,
		a.Tint,
		a.Coatings, -- Added 1/5/18
		a.ODSegHeight,
		a.OSSegHeight,
		a.NumberOfCases,
		a.Pairs,
		a.FrameCode,
		a.FrameColor,
		a.FrameEyeSize,
		a.FrameBridgeSize,
		a.FrameTempleType,
		g.ODPDDistant,
		g.OSPDDistant,
		g.PDDistant,
		g.ODPDNear,
		g.OSPDNear,
		g.PDNear,
		a.ClinicSiteCode,
		c.SiteName AS clinicname,
		e.Address1 AS clinicaddress1,
		e.Address2 AS clinicaddress2,
		e.Address3 as clinicaddress3,
		e.City AS cliniccity,
		e.Country AS cliniccountry,
		e.State AS clinicstate,
		e.ZipCode AS cliniczipcode,
		b.LabSiteCode,
		d.SiteName AS labname,
		f.Address1 AS labaddress1,
		f.Address2 AS labaddress2,
		f.Address3 as labaddress3,
		f.City AS labcity,
		f.Country AS labcountry,
		f.State AS labstate,
		f.ZipCode AS labzipcode,
		a.ShipToPatient,
		a.ShipAddress1,
		a.ShipAddress2,
		a.ShipAddress3,
		a.ShipCity,
		a.ShipState,
		case when LEN(a.ShipZipCode) > 5 then LEFT(a.ShipZipCode,5) end BaseZip,
		case when LEN(a.shipZipCode) > 5 then RIGHT(a.ShipZipCode,4) 
			else '0000'end ZipPlus,
		a.ShipZipCode,
		a.ShipAddressType,
		a.LocationCode,
		Case when @Comment = '' then
			b.StatusComment + '.  ' + a.UserComment1 --as UserComment1,
		else @Comment + '.    ' + a.UserComment1 end UserComment1,
		a.UserComment2 + '.   ' + i.Comments as UserComment2,
        a.IsGEyes,
        a.IsMultivision,
        a.VerifiedBy,
        a.PatientEmail,
        a.DateLastModified AS dateordercreated,
        a.OSDistantDecenter as osdistantdecenter,
        a.ODDistantDecenter as oddistantdecenter,
        a.OSNearDecenter as osneardecenter,
        a.ODNearDecenter as odneardecenter,
        a.ODBase as odbase,
        a.OSBase as osbase,
        g.ODAdd,
        RIGHT('000' + CAST(g.ODAxis AS VarCHAR(3)),3) AS ODAxis,  -- Modified 4/1/15 - baf per user request
        convert(decimal(4,2),round(g.ODCylinder/25.0,2) * 25) as ODCylinder,
        g.ODHBase,
        g.ODHPrism,
        g.ODSphere,
        g.ODVBase,
        g.ODVPrism,
        g.OSAdd,
        RIGHT('000' + CAST(g.OSAxis AS VarCHAR(3)),3) AS OSAxis,  -- Modified 4/1/15 - baf per user request
		convert(decimal(4,2),round(g.OSCylinder/25.0,2) * 25) as  OSCylinder,
        g.OSHBase,
        g.OSHPrism,
        g.OSSphere,
        g.OSVBase,
        g.OSVPrism,
        h.ODCorrectedAcuity,
        h.ODOSCorrectedAcuity,
        h.ODOSUncorrectedAcuity,
        h.ODUncorrectedAcuity,
        h.OSCorrectedAcuity,
        h.OSUncorrectedAcuity,
        h.examdate,
        i.FirstName,
        i.MiddleName,
        i.LastName,
        j.LastName + ', ' + j.FirstName AS doctor,
        @IDNbr as patientidnumber,
        l.AreaCode + l.PhoneNumber as patientphonenumber,
        n.[Text] as BOSDesc,
        o.[text] as StatDesc,
        p.[Text] as OrdPriDesc,
        case
			when q.[Value] = '15' then 'N-15 Drk Grey'
			when q.[Value] = '31' then 'N-31 Med Grey'
			when q.[Value] = '40' then 'Nom. 60% Trans'
	--		when q.[Value] = 'AR' then 'Anti-Refl'
			when q.[Value] = 'CL' then 'Clear'
			when q.[Value] = '51' then 'S-1 Lt Pink'
			when q.[Value] = '52' then 'S-2 Med Pink'
	--		when q.[Value] = 'UV' then 'UV-400'
			WHEN q.[Value] = '60' THEN 'Nom. 40% Trans'
			WHEN q.[Value] = 'P8' THEN 'Pink 85%'
			WHEN q.[Value] = 'P6' THEN 'Pink 60%'
			WHEN q.[Value] = 'A8' THEN 'Amber 80%'
			WHEN q.[Value] = 'A6' THEN 'Amber 60%'
			WHEN q.[Value] = 'B8' THEN 'Blue 85%'
			WHEN q.[Value] = 'B6' THEN 'Blue 65%'
			WHEN q.[Value] = 'FL' THEN 'FL41 50% (Rose)'
        end TintDesc,
        CASE WHEN a.Coatings = 'AR' THEN 'Anti-Refl'
			WHEN a.Coatings = 'UV' THEN 'UV-400'
			WHEN a.Coatings = 'AR/UV' THEN 'Anti-Refl/UV-400'
			WHEN a.Coatings = 'UV/AR' THEN 'UV-400/Anti-Refl'
			ELSE ''
		END CoatingsDesc,
        r.[Text] as ColorDesc,
        s.[Text] as MaterialDesc,
        t.[text] as LensDesc,
        a.ONBarCode,
        a.DispenseComments
    FROM dbo.PatientOrder a 
    INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
    Left Outer JOIN dbo.SiteCode c ON a.ClinicSiteCode = c.SiteCode
    Left Outer JOIN dbo.SiteCode d ON b.LabSiteCode = d.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress e ON a.ClinicSiteCode = e.SiteCode
    LEFT OUTER JOIN dbo.SiteAddress f ON b.LabSiteCode = f.SiteCode
    INNER JOIN dbo.Prescription g ON a.PrescriptionID = g.ID
    LEFT OUTER JOIN dbo.Exam h ON g.ExamID = h.ID
    INNER JOIN dbo.Individual i ON a.IndividualID_Patient = i.ID
    INNER JOIN dbo.Individual j ON g.IndividualID_Doctor = j.ID
    INNER JOIN dbo.IndividualPhoneNumber l ON a.PatientPhoneID = l.ID
    INNER JOIN dbo.Individual m ON a.IndividualID_Tech = m.ID
	inner join dbo.LookupTable n on SUBSTRING(a.Demographic, 4, 1) = n.[Value] -- Branch Of Service
	inner join dbo.LookupTable o on SUBSTRING(a.Demographic, 5, 2) = o.[Value] -- Status Code
	inner join dbo.LookupTable p on SUBSTRING(a.Demographic, 8, 1) = p.[Value] -- Priority Code
	inner join dbo.FrameItem q on a.Tint = q.[Value]
	inner join dbo.FrameItem r on a.FrameColor = r.[Value]
	inner join dbo.FrameItem s on a.LensMaterial = s.[Value]
	inner join dbo.FrameItem t on a.LensType = t.[Value]
	WHERE a.IsActive = 1
	AND b.IsActive = 1
	AND e.AddressType = 'MAIL'
	AND f.AddressType = 'MAIL'
	AND n.Code = 'BOSType'
	and o.Code = 'PatientStatusType'
	AND p.Code = 'OrderPriorityType'
	AND q.TypeEntry = 'Tint'
	AND r.TypeEntry = 'Color'
	AND s.TypeEntry = 'Material'
	AND t.TypeEntry = 'Lens_Type'
	AND q.IsActive = 1
	AND (@OrderNbr Is Null or a.OrderNumber = @OrderNbr)
	ORDER BY a.ClinicSiteCode, a.OrderNumber
	
	Drop Table #OrderPos
END
GO
/****** Object:  StoredProcedure [dbo].[GetPrescriptionsByIndividualID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  12/8/15 - baf COA1 changes
--		16 March 2018 - baf - Added Scanned Rx ID
-- =============================================
CREATE PROCEDURE [dbo].[GetPrescriptionsByIndividualID] 
@IndividualID	INT,
@ModifiedBy		VarChar(200) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	------ Audit MultiRead - 10/17/14 - BAF
	Declare @ReadDate	DateTime = GetDate()
	Declare @Patient	VarChar(20) = Cast(@IndividualID as VarChar(20))
	Declare @Notes		VarChar(Max)
	
	Select @Notes = Coalesce(@Notes, 'Prescription IDs: ') + CAST(ID as VarChar(20)) + ', '
	FROM dbo.Prescription
	WHERE IndividualID_Patient = @IndividualID
	--Added following checks 5/27/14 BAF
	and IsActive = 1 
	
	If @Notes is not Null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 11, @Patient, @Notes
	---------------------------
			CREATE TABLE #Rx (ID INT, RXName VARCHAR(200), OrderHistory VARCHAR(MAX))
		INSERT INTO #Rx
		SELECT ID, rxName,'' FROM dbo.Prescription
		WHERE IndividualID_Patient = @IndividualID 
		
		DECLARE @History AS VARCHAR(MAX),
			@ORxID AS INT = 0,
			@RxID AS int
		
		DECLARE Ord CURSOR
		FOR SELECT ID FROM #Rx
		
		OPEN Ord
		
		FETCH Ord INTO @RxID
		WHILE (@@FETCH_STATUS = 0)
			BEGIN	
				If @ORxID <> @RxID
				BEGIN	
					--SELECT @History = STUFF(( select ', (' + FrameCode + ') ' + CAST(CAST(po.DateLastModified as DATE) AS VARCHAR)					
					SELECT @History = STUFF(( Select History From (
						select distinct ', (' + FrameCode + ') ' + CAST(MONTH(po.DateLastModified) AS VARCHAR(2)) + '/' +
							CAST(DAY(po.DateLastModified) AS VARCHAR(2)) + '/' + RIGHT(CAST(YEAR(po.DateLastModified) AS VARCHAR(4)),2) AS History
						--select distinct ', (' + FrameCode + ') ' + CAST(CAST(po.DateLastModified as DATE) AS VARCHAR)as History
						FROM #Rx
							LEFT JOIN dbo.PatientORder po ON @RxID = po.PrescriptionID
							LEFT JOIN dbo.PatientOrderStatus pos ON pos.OrderNumber = po.OrderNumber
						WHERE po.IndividualID_Patient = @IndividualID 
							AND Year(po.DateLastModified) >= Year(GETDATE())-2
							AND (pos.OrderStatusTypeID NOT IN (14,15) AND pos.IsActive = 1)
							AND (YEAR(po.DateLastModified) >= YEAR(GetDate())-2))x
							--and pos.StatusComment like '%Created%'))x
					for XML PATH(''), TYPE).value('.[1]', 'nvarchar(max)'), 1, 2, '')
				END			
				UPDATE #Rx SET OrderHistory = @History WHERE ID = @RxID
				Set @ORxID = @RxID
				Set @History = Null
				Fetch Ord into @RxID
			END
		
		CLOSE Ord
		DEALLOCATE Ord
		
		SELECT distinct  p.RxName, Cast(p.PrescriptionDate as Date) as RxDate, ODSphere, ODCylinder, 
			RIGHT('000' + CAST(ODAxis AS VarCHAR(3)),3) AS ODAxis,
			ODAdd, ODHPrism, ODVPrism, OSSphere, OSCylinder, 
			RIGHT('000' + CAST(OSAxis AS VarCHAR(3)),3) AS OSAxis,
			OSAdd, OSHPrism, OSVPrism, Rx.OrderHistory as OrderedFrameHistory,
		Case when ExamID IS NULL then 0 else ExamID end ExamID, 
		p.ID,p.IndividualID_Patient, IndividualID_Doctor,	p.ModifiedBy, 	p.DateLastModified,	ODHBase, 	ODVBase, 
		OSHBase, 	OSVBase, 	EnteredODSphere,	EnteredOSSphere,	EnteredODCylinder,	EnteredOSCylinder,	EnteredODAxis,
		EnteredOSAxis,	PrescriptionDate,	p.IsActive,	IsUsed,	PDNear,	PDDistant,	ODPDDistant,	ODPDNear,	OSPDDistant,
		OSPDNear,	Case when IsMonoCalculation IS Null then 0 else IsMonoCalculation end IsMonoCalculation, RxScanID
		FROM dbo.Prescription p
		Inner JOIN #Rx rx ON p.ID = rx.ID
		WHERE p.ID = rx.ID 
		--Added following checks 5/27/14 BAF
		and p.IsActive = 1
		ORDER BY p.PrescriptionDate DESC

		DROP TABLE #Rx

	
END
GO
/****** Object:  StoredProcedure [dbo].[GetPrescriptionOrders]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 7 December 2015
-- Description:	Retrieves all PatientOrders for a provided Prescription ID
-- =============================================
CREATE PROCEDURE [dbo].[GetPrescriptionOrders]
	-- Add the parameters for the stored procedure here
	@RxID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT po.OrderNumber, FrameCode, pos.DateLastModified
	FROM dbo.PatientOrder po
		LEFT JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
	WHERE po.PrescriptionID = @RxID AND YEAR(pos.DatelastModified) >= YEAR(GETDATE())-2
		AND pos.StatusComment LIKE '%Created%'
	ORDER BY pos.DateLastModified desc
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOverdueOrdersFromLabToClinic]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/16/2016
-- Description:	Get Overdue orders from lab checkin to clinic (10 days or more)
--    Part of GetPatientOrderSummaryClinicBySiteCode
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOverdueOrdersFromLabToClinic]
	-- Add the parameters for the stored procedure here
	@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS OverDue 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 2
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND a.OrderNumber = b.OrderNumber
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	--AND b.DateLastModified < DATEADD(Day, 10, GETDATE())
	--And GETDATE() >= DATEADD(Day,10,b.dateLastModified)
	and GetDate() >= dbo.fn_AddBizDays(b.DateLastModified,10)
	GROUP BY a.ClinicSiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrderWithPrescriptionByIndividualID]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*-- =============================================
-- Author:		John Shear
-- Create date: 31 Oct 2011
-- Description:	Returns all order for a patient
-- Modified:  12/2/2014 - BAF - Added SiteCode parameter
--			   1/9/2015 - CCD - Added LabSiteCode OR clause, and ADM002 to IN clause
--			   1/15/2015 - CCD - Removed all sitecode references, filtering out "Reclaim"
--			   9/30/2015 - BAF - added Type Entry parameters to where clause
--			  1/2/2018 - baf - Added Coatings
--			16 March 2018 - baf - Added Scanned RX ID
--			19 March 2018 - baf - Added EmailPatient
-- ============================================= */
CREATE PROCEDURE [dbo].[GetPatientOrderWithPrescriptionByIndividualID] 
	-- Add the parameters for the stored procedure here
@IndividualID	int,
@ModifiedBy		VarChar(200),
@IsGeyes		bit = 0
--@SiteCode		VARCHAR(6) Not required after 1/27/2015, filtering now done in code
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	----------------------
	/*  This Audit Read will contain two insert statements,
		the SP selects the order information as well as
		the order status information
		
		AUDIT Insert Multi Read -- 10/20/14 - BAF
	*/
	Declare @ReadDate	DateTime = GetDate()
	Declare @PatientID	VarChar(200)  = Cast(@IndividualID as VarChar(200))
	Declare @Orders		VarChar(Max)
	Declare @OrderStatus VarChar(Max)
	
	Select @Orders = Coalesce (@Orders,'ORDER Numbers: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
		INNER JOIN dbo.PatientOrderStatus s ON a.OrderNumber = s.OrderNumber and s.IsActive = 1
	WHERE a.IndividualID_Patient = @IndividualID
	and (s.OrderStatusTypeID not in (3,5,14,15,16,17)) 

	If @Orders is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientID, @Orders
	
	Select @OrderStatus = Coalesce (@OrderStatus, 'Order Status: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrderStatus a
	INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber and a.IsActive = 1
	WHERE b.IndividualID_Patient = @IndividualID 
	
	If @OrderStatus is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientID, @OrderStatus
	-----------------------	
	
	--**** WILL USE TABLE BELOW FOR MULTIPLE COATINGS ************
	--CREATE TABLE #Orders (OrderNbr VARCHAR(16), CoatLen INT, Coating VARCHAR(40), DEScrip VARCHAR(250))
	--INSERT INTO #Orders
	--SELECT OrderNumber, LEN(Coatings), Coatings,'' FROM dbo.PatientOrder WHERE IndividualID_Patient = @IndividualID AND Coatings IS NOT null
	
	if @IsGeyes = 1
	BEGIN	
		SELECT 
		a.OrderNumber,
		a.LensType,
		a.FrameCode,
		a.FrameColor,
		a.ClinicSiteCode,
		a.LabSiteCode, 
		a.DateLastModified as DateCreated,
		s.DateLastModified as StatusDate,
		o.OrderStatusDescription,
		a.ODSegHeight,
		a.OSSegHeight,
		b.ODSphere,
		b.ODCylinder,
		b.ODAxis,
		--RIGHT('000' + CAST(b.ODAxis AS VarCHAR(3)),3) AS ODAxis,
		b.ODAdd,
		b.OSSphere,
		b.OSCylinder,
		b.OSAxis,
		--RIGHT('000' + CAST(b.OSAxis AS VarCHAR(3)),3) AS ODAxis,
		b.OSAdd,
		f.FrameDescription, -- Added for G-Eyes DB 02 Dec 2013
		fitem1.[Text] AS 'LensTint', -- Added for G-Eyes DB 02 Dec 2013
		CASE WHEN a.Coatings IS NULL THEN '' 
			WHEN a.Coatings = 'AR' THEN 'Anti-Refl'
			WHEN a.Coatings = 'UV' THEN 'Ultra Violet 400'
			WHEN a.Coatings = 'AR/UV' THEN 'Anti-Refl/Ultra Violet 400'
			WHEN a.Coatings = 'UV/AR' THEN 'Ultra Violet 400/Anti-Refl'
		end 'LensCoating', -- Added 1/2/18
	--	CASE WHEN a.Coatings IS NULL THEN '' ELSE fitem3.[Text] end 'LensCoating', -- Added 1/2/18
		fitem2.[Text] AS 'LensTypeLong', -- Added for G-Eyes DB 02 Dec 2013,
		b.PDNear,
		b.PDDistant,
		b.ODPDDistant,
		b.ODPDNear,
		b.OSPDDistant,
		b.OSPDNear,
		b.RXScanId,
		a.EmailPatient
		FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		INNER JOIN dbo.Frame f ON a.FrameCode = f.FrameCode -- Added for G-Eyes DB 02 Dec 2013
		INNER JOIN dbo.FrameItem fitem1 ON a.Tint = fitem1.Value -- Added for G-Eyes DB 02 Dec 2013
		INNER JOIN dbo.FrameItem fitem2 ON a.LensType = fitem2.Value -- Added for G-Eyes DB 02 Dec 2013
		Left Join dbo.FrameItem fitem3 on a.Coatings = fitem3.value -- added 1/2/18
		INNER JOIN dbo.PatientOrderStatus s ON a.OrderNumber = s.OrderNumber 
		INNER JOIN dbo.OrderStatusType o ON s.OrderStatusTypeID = o.ID
		WHERE a.IndividualID_Patient = @IndividualID
		and (s.OrderStatusTypeID not in (3,5,14,15,17)) -- Added for G-Eyes baf 7/21/14
		AND a.IsActive = 1 and fitem1.TypeEntry = 'TINT' and fitem2.TypeEntry = 'LENS_TYPE'		
		and b.PrescriptionDate > DATEADD(yy,-4,GetDate()) -- Added per OFAB decision Prescriptions over 4 yrs old not good for reorder
		and s.IsActive = 1 AND fitem1.IsActive = 1
		--AND ((a.ClinicSiteCode = @SiteCode) 
		--OR (a.LabSiteCode = @SiteCode)  
		--OR @SiteCode IN ('ADM001', 'ADM002', 'GEyes1' ))
	--	OR fitem1.TypeEntry = 'TINT'
		--and(a.Coatings is null or fitem3.TypeEntry = 'COATING') -- 1/2/18
		ORDER BY a.OrderNumber DESC;
	END
	ELSE
	BEGIN
		SELECT 
		a.OrderNumber,
		a.LensType,
		a.FrameCode,
		a.FrameColor,
		a.ClinicSiteCode,
		a.LabSiteCode, 
		a.DateLastModified as DateCreated,
		s.DateLastModified as StatusDate,
		o.OrderStatusDescription,
		a.ODSegHeight,
		a.OSSegHeight,
		b.ODSphere,
		b.ODCylinder,
		--b.ODAxis,
		RIGHT('000' + CAST(b.ODAxis AS VarCHAR(3)),3) AS ODAxis,
		b.ODAdd,
		b.OSSphere,
		b.OSCylinder,
		--b.OSAxis,
		RIGHT('000' + CAST(b.OSAxis AS VarCHAR(3)),3) AS OSAxis,
		b.OSAdd,
		f.FrameDescription, -- Added for G-Eyes DB 02 Dec 2013
		fitem1.[Text] AS 'LensTint', -- Added for G-Eyes DB 02 Dec 2013
		CASE WHEN a.Coatings IS NULL THEN '' 
			WHEN a.Coatings = 'AR' THEN 'Anti-Refl'
			WHEN a.Coatings = 'UV' THEN 'Ultra Violet 400'
			WHEN a.Coatings = 'AR/UV' THEN 'Anti-Refl/Ultra Violet 400'
			WHEN a.Coatings = 'UV/AR' THEN 'Ultra Violet 400/Anti-Refl'
		end 'LensCoating', -- Added 1/2/18
	--	CASE WHEN a.Coatings IS NULL THEN '' ELSE fitem3.[Text] end 'LensCoating', -- Added 1/2/18
		fitem2.[Text] AS 'LensTypeLong', -- Added for G-Eyes DB 02 Dec 2013
		b.PDNear,
		b.PDDistant,
		b.ODPDDistant,
		b.ODPDNear,
		b.OSPDDistant,
		b.OSPDNear,
		b.RXScanId,
		a.EmailPatient
		FROM dbo.PatientOrder a
		INNER JOIN dbo.Prescription b ON a.PrescriptionID = b.ID
		LEFT JOIN dbo.Frame f ON a.FrameCode = f.FrameCode -- Added for G-Eyes DB 02 Dec 2013
		LEFT JOIN dbo.FrameItem fitem1 ON a.Tint = fitem1.Value -- Added for G-Eyes DB 02 Dec 2013
		LEFT JOIN dbo.FrameItem fitem2 ON a.LensType = fitem2.Value -- Added for G-Eyes DB 02 Dec 2013
		Left Join dbo.FrameItem fitem3 on a.Coatings = fitem3.Value -- Added 1/2/18
		INNER JOIN dbo.PatientOrderStatus s ON a.OrderNumber = s.OrderNumber 
		INNER JOIN dbo.OrderStatusType o ON s.OrderStatusTypeID = o.ID
		WHERE a.IndividualID_Patient = @IndividualID
			AND (s.OrderStatusTypeID <> 17) AND a.IsActive = 1 AND s.IsActive = 1
			and fitem1.TypeEntry = 'TINT' and fitem2.TypeEntry = 'LENS_TYPE'	
			AND fitem1.IsActive = 1
			--and fitem3.TypeEntry = 'COATING'	
	--		and(a.Coatings is null or fitem3.TypeEntry = 'COATING') -- 1/2/18
		ORDER BY a.OrderNumber DESC;
	END
	SELECT
	a.OrderNumber, 
	a.DateLastModified, 
	a.IsActive, 
	a.LabSiteCode,
	b.ClinicSiteCode,
	a.ModifiedBy, 
	a.OrderStatusTypeID, 
	a.StatusComment, 
	d.OrderStatusDescription,
	s.SiteName,
	s.RegPhoneNumber
	FROM dbo.PatientOrderStatus a
	INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber and a.IsActive = 1
	INNER JOIN dbo.OrderStatusType d ON a.OrderStatusTypeID = d.ID
	INNER JOIN dbo.SiteCode s ON a.LabSiteCode = s.SiteCode
	WHERE b.IndividualID_Patient = @IndividualID
		AND (a.OrderStatusTypeID <> 17) AND b.IsActive = 1
	ORDER BY a.OrderNumber, a.DateLastModified DESC;

	--DROP TABLE #Orders

END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersWithProblemsByClinicCode]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  7/1/2015 baf to remove check for On Hold Orders
--		3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersWithProblemsByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	---------- AUDIT MutliRead 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + CAST(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
	INNER JOIN dbo.OrderStatusType d ON b.OrderStatusTypeID = d.ID
	WHERE (b.OrderStatusTypeID = 5 OR b.OrderStatusTypeID = 3 OR b.OrderStatusTypeID = 15) -- OR b.OrderStatusTypeID = 16) 
	AND a.ClinicSiteCode = @ClinicSiteCode
	AND b.IsActive = 1
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	
	SELECT @Notes = COALESCE(@Notes, 'Problem Order Numbers: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
	INNER JOIN dbo.OrderStatusType d ON b.OrderStatusTypeID = d.ID
	WHERE (b.OrderStatusTypeID = 5 OR b.OrderStatusTypeID = 3 OR b.OrderStatusTypeID = 15) --OR b.OrderStatusTypeID = 16) 
	AND a.ClinicSiteCode = @ClinicSiteCode
	AND b.IsActive = 1
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	-----------------------------------------

	SELECT a.OrderNumber,
	b.OrderStatusTypeID,
	d.OrderStatusDescription,
	b.StatusComment,
	a.ClinicSiteCode, 
	b.LabSiteCode, 
	b.IsActive,
	b.ModifiedBy,
	b.DateLastModified,
	a.DateLastModified AS 'DateOrderCreated',
	a.FrameCode,
	a.LensType,
	a.LensMaterial, 
	c.ID AS IndividualID, -- added 6/8/2017
	c.LastName,
	c.MiddleName,
	c.FirstName,
	a.EmailPatient
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	INNER JOIN dbo.Individual c ON a.IndividualID_Patient = c.ID
	INNER JOIN dbo.OrderStatusType d ON b.OrderStatusTypeID = d.ID
	WHERE (b.OrderStatusTypeID = 5 OR b.OrderStatusTypeID = 3 OR b.OrderStatusTypeID = 15) -- OR b.OrderStatusTypeID = 16) 
	AND a.ClinicSiteCode = @ClinicSiteCode
	AND b.IsActive = 1
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	ORDER BY b.OrderStatusTypeID, a.OrderNumber
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrderStatusPresentCodeByOrderNumber]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  3 May 2016 - baf - modified to pull the statuses for
--		multiple orders if supplied in a comma delimited list of order numbers.
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrderStatusPresentCodeByOrderNumber] 
	@OrderNumber		VARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID	INT = 0,
		@ShipToPatient BIT,
		@RecCnt INT,
		@Order VARCHAR(16),
		@CurRec INT = 1,
		@OrderSTatus int,
		@Comment VARCHAR(500),
		@LabSiteCode VARCHAR(6), 
		@ISActive  bit,
		@StatusDesc  VarChar(250),
		@DateLastMOdified DateTime;

	BEGIN TRY
	
		-- Need to loop through the order numbers if there is more than one
		-- Added 3/31/15 -- Check-In/Check-Out Changes

		CREATE TABLE #tmpOrderNbrs
			(RowID int, OrderNbr VarChar(16), OrderStatusTypeID int, StatusComment VarChar(500),
			LabSiteCode VarChar(6), IsActive Bit, StatusDescription VarChar(250), DateLastModified DateTime)
			
		INSERT INTO #tmpOrderNbrs (RowID, OrderNbr)
		SELECT * FROM dbo.SplitStrings(@OrderNumber,',')	

		--SELECT * FROM #tmpOrderNbrs;
		
		Select @RecCnt = MAX(RowID) FROM #tmpOrderNbrs;
		SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		
		--ALTER TABLE #tmpOrderNbrs
		--ADD OrderStatusTypeID INT,
		--	StatusComment VARCHAR(100),
		--	LabSiteCode VARCHAR(6),
		--	IsActive BIT,
		--	StatusDescription VarChar(250),
		--	DateLastModified DATETIME
		
		while @RecCnt >= @CurRec 
		Begin
			BEGIN TRANSACTION
			SET NOCOUNT OFF;
			
			SELECT @OrderSTatus = pos.OrderStatusTypeID, @Comment = pos.StatusComment,
				@LabSiteCode = pos.LabSiteCode,  @ISActive = pos.IsActive,
				@StatusDesc = ost.OrderStatusDescription, @DateLastMOdified = pos.DateLastModified
			FROM dbo.PatientOrderStatus pos
				INNER JOIN dbo.OrderStatusType ost ON pos.OrderStatusTypeID = ost.ID
			WHERE pos.OrderNumber = @Order AND pos.IsActive = 1
			
			UPDATE #tmpOrderNbrs SET OrderStatusTypeID = @OrderSTatus, 
				StatusComment = @Comment, LabSiteCode = @LabSiteCode,
				IsActive = @ISActive, StatusDescription = @StatusDesc,
				DateLastModified = @DateLastMOdified
			where OrderNbr = @Order
			        	
		    SET NOCOUNT ON;
			COMMIT TRANSACTION
			SET @CurRec = @CurRec + 1;
			SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		End
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
	SELECT OrderNbr AS OrderNumber, OrderStatusTypeID, StatusComment, LabSiteCode,IsActive,
		 StatusDescription, DateLastModified FROM #tmpOrderNbrs;
	
	DROP TABLE #tmpOrderNbrs;			
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersSummaryLabBySiteCode]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersSummaryLabBySiteCode]
@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Orders ready for CheckIn from Lab to Clinic
	
	SELECT po.LabSiteCode, COUNT(po.LabSiteCode) AS ReadyForCheckin 
	FROM PatientOrder po
	INNER JOIN PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
	INNER JOIN dbo.Individual k ON po.IndividualID_Patient = k.ID
	WHERE 
	(pos.IsActive = 1 and pos.OrderStatusTypeID in (1, 9, 4, 5, 6, 10))
	AND (po.IsActive = 1 AND po.LabSiteCode = @SiteCode)
	AND k.IsActive = 1
	GROUP BY po.LabSiteCode
	--SELECT a.LabSiteCode, COUNT(a.LabSiteCode) AS ReadyForCheckin 
	--FROM 
	--(SELECT ROW_NUMBER() OVER(PARTITION BY OrderNumber ORDER BY DateLastModified DESC) 
	--AS Seq,* FROM dbo.PatientOrderStatus) t
	--INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber
	--WHERE Seq = 1 
	--AND (t.OrderStatusTypeID = 1 
	--OR t.OrderStatusTypeID = 9 
	--OR t.OrderStatusTypeID = 4
	--OR t.OrderStatusTypeID = 6)
	--AND a.IsActive = 1 
	--And t.IsActive = 1
	--AND t.LabSiteCode = @SiteCode
	--GROUP BY a.LabSiteCode


	/* Average time to dispense - dates between date created and date dispensed average*/
	SELECT UPPER(@SiteCode) AS LabSiteCode, AVG(DATEDIFF(DAY, startdate, enddate)) AS AvgProductionDays
	from (
    SELECT a.DateLastModified AS startdate, a.OrderNumber AS ONumber
    FROM   dbo.PatientOrderStatus a
    INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber
    WHERE OrderStatusTypeID in (2)
    AND a.LabSiteCode = @SiteCode
    AND a.OrderNumber = b.OrderNumber
    AND b.IsActive = 1
	) as query1
	join (
    SELECT c.DateLastModified AS enddate, c.OrderNumber AS ONumber
    FROM   dbo.PatientOrderStatus c
    INNER JOIN dbo.PatientOrder d ON c.OrderNumber = d.OrderNumber
    WHERE OrderStatusTypeID in (7,19) 
    AND d.LabSiteCode = @SiteCode
    AND c.OrderNumber = d.OrderNumber
    AND d.IsActive = 1
	) as query2 ON query1.startdate < query2.enddate AND query1.ONumber = query2.ONumber

	-- gets the number of orders ready for dispense
	--SELECT a.LabSiteCode, COUNT(a.LabSiteCode) AS ReadyForDispense 
	--FROM 
	--(SELECT ROW_NUMBER() OVER(PARTITION BY OrderNumber ORDER BY DateLastModified DESC) 
	--AS Seq,* FROM dbo.PatientOrderStatus) t
	--INNER JOIN dbo.PatientOrder a ON t.OrderNumber = a.OrderNumber
	--WHERE Seq = 1 
	--AND t.OrderStatusTypeID = 2 
	--AND t.LabSiteCode = @SiteCode
	--AND a.IsActive = 1
	--GROUP BY a.LabSiteCode
	SELECT po.LabSiteCode, count(po.LabSiteCode) as ReadyForDispense
	FROM PatientOrder po
	INNER JOIN PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	WHERE 
	(pos.IsActive = 1 and pos.OrderStatusTypeID IN (2)) and 
	po.LabSiteCode = @SiteCode AND
	po.IsActive = 1
	GROUP BY po.LabSiteCode
	
	-- Get number of orders for Hold For Stock (Pending)
	SELECT po.LabSiteCode, COUNT(po.LabSiteCode) AS HoldForStock
	FROM dbo.PatientOrder po
		INNER JOIN dbo.PatientOrderStatus pos ON po.OrderNumber = pos.OrderNumber
	WHERE --((pos.OrderStatusTypeID in (2,5)) or (pos.OrderStatusTypeID = 7 and po.ShipToPatient = 0))
		(pos.IsActive = 1 AND pos.OrderStatusTypeID = 5) --AND Hold For Stock Only
		and po.LabsiteCode = @SiteCode AND po.IsActive = 1
	GROUP BY po.LabSiteCode
		
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersSummaryClinicBySiteCode]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
 =============================================
 Author:		<Author,,Name>
 Create date: <Create Date,,>
 Description:	<Description,,>
 Modified:  7/22/15 - baf - Removed check for OrderStatusTypeID 16 (Hold)
			25 APR 16 - baf - Added Status 16 - Clinic Checkin NO Lab Checkout
			17 AUG 17 - baf - Added Status 2 - Clinic orders at Lab
			30 NOV 17 - baf - Ensured Status 5 only in Problem Orders
 =============================================
*/
CREATE PROCEDURE [dbo].[GetPatientOrdersSummaryClinicBySiteCode]
@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Orders ready for CheckIn from Lab to Clinic
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS ReadyForCheckin 
    FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	inner join dbo.SiteCode sc on sc.SiteCode = b.labsitecode
	WHERE b.OrderStatusTypeID = 7
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	and a.ShipToPatient = 0
	--and sc.ShipToPatientLab = 0
	AND NOT b.StatusComment = 'Lab Dispensed And Shipped Order To Patient'
	GROUP BY a.ClinicSiteCode
	
	-- Orders rejected by Lab
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS Rejected 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE (b.OrderStatusTypeID = 3 OR b.OrderStatusTypeID = 5 OR b.OrderStatusTypeID = 15)-- OR b.OrderStatusTypeID = 16)
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	GROUP BY a.ClinicSiteCode
	
	-- Orders where production complete is 10 or more days greater than date lab checked in
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS OverDue 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID in (2)
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND a.OrderNumber = b.OrderNumber
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	--AND b.DateLastModified < DATEADD(Day, 10, GETDATE())
	--And GETDATE() >= DATEADD(Day,10,b.dateLastModified)
	--	and GetDate() >= dbo.fn_AddBizDays(b.DateLastModified,10)
	and DATEDIFF(DAY, dbo.fn_AddBizDays(b.DateLastModified,10), GETDATE()) >= 1
	GROUP BY a.ClinicSiteCode
	
	-- Average time to dispense - dates between date created and date dispensed average
	SELECT @SiteCode AS ClinicSiteCode, AVG(DATEDIFF(DAY, startdate, enddate)) AS AvgDispenseDays
	from (
    SELECT a.DateLastModified AS startdate, a.OrderNumber AS ONumber
    FROM   dbo.PatientOrderStatus a
    INNER JOIN dbo.PatientOrder b ON a.OrderNumber = b.OrderNumber
    WHERE OrderStatusTypeID = 1
    AND b.ClinicSiteCode = @SiteCode
    AND a.OrderNumber = b.OrderNumber
    AND b.IsGEyes = 0
    AND b.IsActive = 1 and b.ModifiedBy <> 'SSIS'
	) as query1
	join (
    SELECT c.DateLastModified AS enddate, c.OrderNumber AS ONumber
    FROM   dbo.PatientOrderStatus c
    INNER JOIN dbo.PatientOrder d ON c.OrderNumber = d.OrderNumber
    WHERE c.OrderStatusTypeID = 8 
    AND d.ClinicSiteCode = @SiteCode
    AND c.OrderNumber = d.OrderNumber
    AND d.IsActive = 1 and d.ModifiedBy <> 'SSIS'
    AND d.IsGEyes = 0
	) as query2 ON query1.startdate < query2.enddate AND query1.ONumber = query2.ONumber
	
	-- gets the number of orders ready for dispense
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS ReadyForDispense 
    FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID IN (11,16)
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND b.OrderNumber = a.OrderNumber
	AND a.IsGEyes = 0
	AND a.IsActive = 1
	GROUP BY a.ClinicSiteCode
	
	-- Order Count for orders currently checked-in at lab.
	Select po.CLinicSiteCode, COUNT(po.OrderNumber) as OrdersAtLab
	From dbo.PatientOrder po
		Inner Join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	where po.IsActive = 1 and pos.IsActive =1
		and ((pos.OrderStatusTypeID in (2,5)) or (pos.OrderStatusTypeID = 7 and po.ShipToPatient = 0))
		and ClinicSiteCode = @SiteCode
	Group by ClinicSiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersRejectedByLab]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/16/2016
-- Description:	Get Patient Orders that were rejected by the lab
--    Part of GetPatientOrderSummaryClinicBySiteCode
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersRejectedByLab]
	-- Add the parameters for the stored procedure here
	@SiteCode	VARCHAR(6) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT a.ClinicSiteCode, COUNT(a.ClinicSiteCode) AS Rejected 
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE (b.OrderStatusTypeID = 3 OR b.OrderStatusTypeID = 5 OR b.OrderStatusTypeID = 15)-- OR b.OrderStatusTypeID = 16)
	AND b.IsActive = 1 
	AND a.ClinicSiteCode = @SiteCode
	AND a.IsActive = 1
	AND a.IsGEyes = 0
	GROUP BY a.ClinicSiteCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetPatientOrdersOnHoldByClinicCode]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPatientOrdersOnHoldByClinicCode] 
@ClinicSiteCode	VARCHAR(6),
@ModifiedBy		VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	---------- Audit MultiRead 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + Cast(a.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 16 
	AND b.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers on Hold: ') + a.OrderNumber + ', '
	FROM dbo.PatientOrder a
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 16 
	AND b.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
	IF @Notes IS NOT NULL
		EXEC InsertAuditMultiRead @ReadDate,  @ModifiedBy, 12, @PatientList, @Notes
	----------------------------------------
	
	SELECT * FROM dbo.PatientOrder a
	INNER JOIN dbo.Prescription pr ON pr.ID = a.PrescriptionID
	INNER JOIN dbo.PatientOrderStatus b ON a.OrderNumber = b.OrderNumber
	WHERE b.OrderStatusTypeID = 16 
	AND b.IsActive = 1 and a.IsActive = 1
	AND a.ClinicSiteCode = @ClinicSiteCode
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetWoundedWarriorReportData]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  1/3/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetWoundedWarriorReportData]
@FromDate	DATETIME,
@ToDate	DATETIME,
@ClinicSiteCode varchar(6) = null,
@LabSiteCode varchar (6) = null,
@ModifiedBy		VarChar(200)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--------------Audit MultiRead 10/21/14 - BAF
	Declare @ReadDate		DateTime = GetDate()
	Declare @PatientList	VarChar(Max)
	Declare @Notes			VarChar(Max)
	
	Select @PatientList = Coalesce(@PatientList, 'PatientIDs: ') + Cast(po.IndividualID_Patient as VarChar(20)) + ', '
	FROM dbo.PatientOrder po Inner Join  
		(Select Row_Number() Over (Partition By IndividualID Order By DateLastModified desc) as RowID, ID, IndividualID, IDNumber, 
			IsActive,Max(DateLastModified) as MaxDate
			from dbo.IndividualIDNumber id
			Group by ID, IndividualID, IDNumber, IsActive, DateLastModified) id
		on po.IndividualID_Patient = id.IndividualID
		left join Individual ind on id.IndividualID = ind.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	WHERE SUBSTRING(po.demographic, 8, 1) = 'W'
		and pos.OrderStatusTypeID not in ('5','14')
		and pos.IsActive = 1
		AND po.DateLastModified BETWEEN @FromDate and @ToDate
		and (@ClinicSiteCode is null or po.ClinicSiteCode = @ClinicSiteCode)
		and (@LabSiteCode is null or po.LabSiteCode = @LabSiteCode)
		and id.IsActive = 1 and Id.RowID = 1
		and id.MaxDate <= GETDATE()
		
	Select @Notes = Coalesce(@Notes, 'Wounded Warrior Report OrderNumbers: ') + po.OrderNumber + ', '
	FROM dbo.PatientOrder po Inner Join  
		(Select Row_Number() Over (Partition By IndividualID Order By DateLastModified desc) as RowID, ID, IndividualID, IDNumber, 
			IsActive,Max(DateLastModified) as MaxDate
			from dbo.IndividualIDNumber id
			Group by ID, IndividualID, IDNumber, IsActive, DateLastModified) id
		on po.IndividualID_Patient = id.IndividualID
		left join Individual ind on id.IndividualID = ind.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	WHERE SUBSTRING(po.demographic, 8, 1) = 'W'
		and pos.OrderStatusTypeID not in ('5','14')
		and pos.IsActive = 1
		AND po.DateLastModified BETWEEN @FromDate and @ToDate
		and (@ClinicSiteCode is null or po.ClinicSiteCode = @ClinicSiteCode)
		and (@LabSiteCode is null or po.LabSiteCode = @LabSiteCode)
		and id.IsActive = 1 and Id.RowID = 1
		and id.MaxDate <= GETDATE()
		
	If @PatientList is not null
		Exec InsertAuditMultiRead @ReadDate, @ModifiedBy, 10, @PatientList, @Notes
	--------------------------------------------

	SELECT distinct po.OrderNumber,right(id.IDNumber,4) as LastFour,  ind.LastName, ind.FirstName,
		SUBSTRING(ind.MiddleName,1,1) as MI, po.ClinicSiteCode, po.LabSiteCode, 
		po.FrameCode, po.LensType, po.LensMaterial, po.Tint, po.Coatings, 
		po.DateLastModified as OrderDate, pos.OrderStatusTypeID, pos.IsActive, po.Demographic
	FROM dbo.PatientOrder po Inner Join  
		(Select Row_Number() Over (Partition By IndividualID Order By DateLastModified desc) as RowID, ID, IndividualID, IDNumber, 
			IsActive,Max(DateLastModified) as MaxDate
			from dbo.IndividualIDNumber id
			Group by ID, IndividualID, IDNumber, IsActive, DateLastModified) id
		on po.IndividualID_Patient = id.IndividualID
		left join Individual ind on id.IndividualID = ind.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
	WHERE SUBSTRING(po.demographic, 8, 1) = 'W'
		and pos.OrderStatusTypeID not in ('5','14')
		and pos.IsActive = 1
		AND po.DateLastModified BETWEEN @FromDate and DATEADD(d,+1,@ToDate)
		and (@ClinicSiteCode is null or po.ClinicSiteCode = @ClinicSiteCode)
		and po.LabSiteCode not like 'SPURS%' 
		and (@LabSiteCode is null or po.LabSiteCode = @LabSiteCode)
		and id.IsActive = 1 and Id.RowID = 1
		and id.MaxDate <= GETDATE()
	ORDER BY po.DateLastModified Desc, po.LabSiteCode, po.OrderNumber
	
END
GO
/****** Object:  StoredProcedure [dbo].[GetUnitDispenseRpt]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified: 1/5/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetUnitDispenseRpt]
	-- Add the parameters for the stored procedure here
	@FromDate DateTime, 
	@ToDate DateTime,
	@SiteCode VarChar(6) = Null,
	@UIC VarChar (6) = Null,
	@Status VarChar(2) = Null,
	@Priority VarChar(1) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		SELECT distinct po.ClinicSiteCode, po.LabSiteCode, ind.LastName, ind.FirstName, 
			SUBSTRING(ind.MiddleName,1,1) as MI, RIGHT(id.IDNumber,4) as LastFour,
			po.OrderNumber, po.FrameCode, Pairs, LensType, Tint, Coatings, -- Added 1/5/18
			SUBSTRING(po.demographic,5,2) as StatusCode,
			SUBSTRING(po.demographic,8,1) as PriorityCode,
			SUBSTRING(po.demographic,4,1) as BOS,
			REPLACE(SUBSTRING(po.demographic,1,3),'*','') as MilRank,
			ia.UIC, pos.DateLastModified as OrdDte, posr.dateLastModified as recDte,
			posd.DateLastModified as DisDte, po.ONBarCode
		FROM dbo.PatientOrder po Left Join  dbo.IndividualIDNumber id
				on po.IndividualID_Patient = id.IndividualID
			left join Individual ind on id.IndividualID = ind.ID
			inner join dbo.IndividualAddress ia on ia.IndividualID = ind.ID
			inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
			inner join dbo.PatientOrderStatus posr on po.OrderNumber = posr.OrderNumber
			inner join dbo.PatientOrderStatus posd on po.OrderNumber = posd.OrderNumber
		Where pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)
			and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status, SUBSTRING(po.Demographic,5,2)) 
			and SUBSTRING(po.Demographic,8,1)= ISNULL(@Priority, SUBSTRING(po.Demographic,8,1)) 
			and pos.OrderStatusTypeID in (1,6) -- GEyes or Clinic Ordered
			and posd.OrderStatusTypeID in (7,8,19) -- Clinic or Lab Dispensed
			and posr.OrderStatusTypeID  = 11 -- Clinic Received Order from Lab
			and posr.DateLastModified > pos.DateLastModified
			and posd.DateLastModified < posr.DateLastModified
			and (@SiteCode is null or po.ClinicSiteCode = @SiteCode)
			and (@UIC is null or ia.UIC = @UIC)
			and id.IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetTurnAroundTimeRpt]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 8/19/2013
-- Description:	This procedure pulls the data for
--		the Turn Around Time Report
-- Modified:  7/9/15 - baf - correct calculations
--			 25 APR 16 - baf - Added Status 16 - Clinic Checkin No Lab Checkout
-- =============================================
CREATE PROCEDURE [dbo].[GetTurnAroundTimeRpt]
	-- Parameters
	@FromDate datetime,
	@ToDate datetime,
	@SiteCode	VARCHAR(6) = null,
	@Status varchar(2) = null,
	@Priority varchar(1) = null
	
AS
	
BEGIN
	 --SET NOCOUNT ON added to prevent extra result sets from
	 --interfering with SELECT statements.
	SET NOCOUNT ON;
	select distinct LabSiteCode, SiteName, ClinicSiteCode, SUM(TotMil) as GTMil, SUM(TotFOC) as GTFOC,
		SUM(TotInserts) as GTInserts, SUM(Tinted) as GTTnoInserts, SUM(woInserts) as GTNoTintIns,
		SUM(TotFOC + TotInserts + Tinted + woInserts) as GrandTotal,
		Case when AVG(wkDayNoInserts) <= 0 then 0 else AVG(wkDayNoInserts) end AvgNoInserts,
		case when AVG(wkDayTint) <= 0 then 0 else AVG(wkDayTint) end AvgTint,
		Case when AVG(WkDayInserts) <= 0 then 0 else AVG(WkDayInserts) end AvgInserts,
		Case when AVG(wkDayFOC) <= 0 then 0 else AVG(wkDayFOC) end AvgFOC,
		case when MIN(WkDayNoInserts) <= 0 then 0 else MIN(wkDayNoInserts) end MinNoInserts,
		case when MIN(wkDayTint) <= 0 then 0 else MIN(wkDayTint) end MinTint,
		case when MIN(wkDayInserts) <= 0 then 0 else MIN(wkDayInserts) end MinInserts,
		Case when MIN(wkDayFOC) <= 0 then 0 else MIN(wkDayFOC) end MinFOC,
		case when MAX(wkDayNoInserts) <= 0 then 0 else MAX(wkDayNoInserts) end MaxNoInserts,
		case when MAX(wkDayTint) <= 0 then 0 else MAX(wkDayTint) end MaxTint,
		Case when MAX(wkDayInserts) <= 0 then 0 else MAX(wkDayInserts) end MaxInserts,
		Case when MAX(wkDayFOC) <= 0 then 0 else MAX(wkDayFOC) end MaxFOC,
		case when SUM((wkDayNoInserts * .85)/100) <= 0 then 0 else SUM((wkDayNoInserts * .85)/100) end NoInserts85,
		case when sum((wkDayTint * .85)/100) <= 0 then 0 else sum((wkDayTint * .85)/100) end Tint85,
		case when SUM((wkDayInserts * .85)/100) <= 0 then 0 else SUM((wkDayInserts * .85)/100) end Inserts85,
		Case when SUM((wkDayFOC * .85)/100) <= 0 then 0 else SUM((wkDayFOC * .85)/100) end FOC85,
		case when SUM((wkDayNoInserts * .95)/100) <= 0 then 0 else SUM((wkDayNoInserts * .95)/100) end NoInserts95,
		case when sum((wkDayTint * .95)/100) <= 0 then 0 else sum((wkDayTint * .95)/100) end Tint95,
		case when SUM((wkDayInserts * .95)/100) <= 0 then 0 else SUM((wkDayInserts * .95)/100) end Inserts95,
		Case when SUM((wkDayFOC * .95)/100) <= 0 then 0 else SUM((wkDayFOC * .95)/100) end FOC95
	from
	(
	select c.OrderNumber,c.LabSiteCode, c.SiteName, c.LabRecDate,c.DispenseDate,orders.ClinicSiteCode, SUM(orders.NbrFrame) as TotMil, SUM(Orders.FOC) as TotFOC, 
			SUM(Orders.Inserts) as TotInserts, SUM(orders.TNoInserts) as Tinted, SUM(orders.NoTintIns) as woInserts, statuscode,orderpriority,
			Case When Orders.Inserts > 0 then dbo.fnWrkDays(c.LabRecDate,c.DispenseDate)  else 0 End wkDayInserts,
			Case when Orders.FOC > 0 then dbo.fnWrkDays(c.labRecDate, c.DispenseDate)  else 0 End wkDayFOC,
			Case when Orders.TNoInserts > 0 then dbo.fnWrkDays(c.LabRecDate, c.DispenseDate)  else 0 End wkDayTint,
			Case When Orders.NoTintIns > 0 then dbo.fnWrkDays(c.labRecDate, c.dispenseDate)  else 0 end wkDayNoInserts
	from
(Select distinct a.OrderNumber, a.LabSiteCode, a.StatusComment as aStatus, b.StatusComment as bStatus, a.DateLastModified as DispenseDate, 
	b.DateLastModified as LabRecDate, scl.SiteName
from
	dbo.PatientOrderStatus a
	join dbo.PatientOrderStatus b on a.OrderNumber = b.OrderNumber
	inner join dbo.SiteCode scl on scl.SiteCode = a.LabSiteCode
	where (a.OrderStatusTypeID in (11,16) or a.OrderStatusTypeID = 19) and
		b.OrderStatusTypeID = 2
	and a.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)
	and LEFT(a.OrderNumber,6) = @SiteCode
)c 
Inner join 
(
Select * from (
select SUBSTRING(po.Demographic, 5,2) AS statuscode, ClinicSiteCode, po.LabSiteCode as LabCode, sc.SiteName, Tint, LensType, LensMaterial, po.FrameCode,
	po.OrderNumber, pos.OrderStatusTypeID, pos.StatusComment, pos.DateLastModified, 
	COUNT(po.FrameCode) as NbrFrame, 
	--case when fc.FrameNotes like '%FOC%' then 1 else 0 end FOC, 
	case when fc.IsFOC = 1 then 1 else 0 end FOC,
	case when fc.IsInsert = 1 then 1 else 0 end Inserts,
	case when (fc.IsInsert = 0 and Tint <> 'CL') then 1 else 0 end TNoInserts,
	case when (fc.IsInsert = 0 and Tint = 'CL') then 1 else 0 end NoTintIns,
	--case when fc.FrameDescription like '%INSERT%' then 1 else 0 end Inserts,
	--case when (fc.FrameDescription not like '%INSERT%' and Tint <> 'CL') then 1 else 0 end TNoInserts,
	--case when (fc.FrameDescription not like '%INSERT%' and Tint = 'CL') then 1 else 0 end NoTintIns,
	IsNull(fc.FrameCode,'') as FocFrame, 
	REPLACE(SUBSTRING(po.Demographic, 1, 3), '*', '') AS rankcode,
	SUBSTRING(po.Demographic, 8, 1) AS orderpriority,
	SUBSTRING(po.Demographic, 4, 1) AS bos
from dbo.PatientOrder po inner join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	 join dbo.Frame fc on po.FrameCode = fc.FrameCode
	Left join dbo.SiteCode sc on po.LabSiteCode = sc.SiteCode
where --SUBSTRING(po.Demographic, 5,2) in (11,12,14,15,21,72,74) and
	 pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate) --) a -- Change Dates to parameters
	and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status,Substring(po.Demographic,5,2))
	and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority,SUBSTRING(po.Demographic,8,1))
	and po.ClinicSiteCode = @SiteCode and po.IsActive = 1 and pos.IsActive = 1
Group by Demographic, ClinicSiteCode, po.LabSiteCode, pos.LabSiteCode,Tint, LensMaterial,LensType,po.OrderNumber,
pos.OrderStatusTypeID, pos.StatusComment, pos.DateLastModified, po.FrameCode, fc.FrameDescription, --fc.FrameNotes,
 sc.SiteCode, SiteName,fc.FrameCode, fc.IsFOC, fc.IsInsert
 )a where StatusCode in (11,12,14,15,16,21,72,74)
) orders
on orders.OrderNumber = c.OrderNumber
group by c.OrderNumber, c.DispenseDate,c.LabRecDate,c.LabSiteCode,orders.ClinicSiteCode, orders.statuscode,
orders.orderpriority, Orders.Inserts, Orders.FOC, Orders.TNoInserts, Orders.NoTintIns, c.SiteName
) Nbrs
where ClinicSiteCode = @SiteCode
group by  LabSiteCode, ClinicSiteCode, SiteName

END
GO
/****** Object:  StoredProcedure [dbo].[GetTurnAroundTime_Totals]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetTurnAroundTime_Totals]
	@FromDate DateTime,
	@ToDate DateTime,
	@SiteCode VarChar(6) = Null,
	@Priority VarChar(1) = Null,
	@Status VarChar(2) = Null
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

select LabCode, LabName, SUM(TotNbrRec)as TotMIL,sum(FOC) as TotFOC, sum(Inserts) as TotInserts, sum(NoInserts) as woInserts, sum(wTint) as Tinted, 
	sum(noTint) as woTint, statuscode,orderpriority
from 
(
	select SUBSTRING(po.Demographic, 5,2) AS statuscode,ClinicSiteCode, po.LabSiteCode as LabCode, Lab.SiteName as LabName, Tint, LensType, 
		LensMaterial, po.FrameCode,	po.OrderNumber, a.OrderStatusTypeID,a.StatusComment, a.DateLastModified, COUNT(a.OrderNumber) as TotNbrRec,
		COUNT(po.FrameCode) as NbrFrame, 
		case when fc.IsFOC = 1 then 1 else 0 end FOC, 
		case when ins.IsInsert = 1 then 1 else 0 end Inserts,
		case when ins.IsInsert = 0 then 0 else 1 end NoInserts,
		case when Tint IS null then 0 else 1 end wTint,
		case when Tint Is null then 1 else 0 end noTint,
		IsNull(fc.FrameCode,'') as FocFrame, isNull(ins.FrameCode,'') as InsFrame,REPLACE(SUBSTRING(po.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(po.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(po.Demographic, 4, 1) AS bos
	from dbo.PatientOrder po right join 
	(select OrderNumber, OrderStatusTypeID, StatusComment, LabSiteCode, IsActive, DateLastModified
	from dbo.PatientOrderStatus
	where OrderStatusTypeID = 2 and IsActive = 1 and
	DateLastModified between @FromDate and @ToDate) a 
	on a.OrderNumber = po.OrderNumber
	left join
			--(Select * from dbo.Frame where FrameNotes like '%FOC%') fc on po.FrameCode = fc.FrameCode
			(Select * from dbo.Frame where IsFOC = 1 and IsActive = 1) fc on po.FrameCode = fc.FrameCode
			left join 
			--(Select * from dbo.Frame where FrameDescription like '%INSERT%') ins on po.FrameCode = ins.FrameCode
			(Select * from dbo.Frame where IsInsert = 1 and IsActive = 1) ins on po.FrameCode = ins.FrameCode
	left join 
	(Select SiteName, SiteCode from dbo.SiteCode) Lab on po.LabSiteCode = Lab.SiteCode
	where SUBSTRING(po.Demographic, 5,2) in (11,12,14,15,21,72,74)
	Group by Demographic, ClinicSiteCode, po.LabSiteCode, a.LabSiteCode,Tint, LensMaterial,LensType,po.OrderNumber,
	a.OrderStatusTypeID, a.StatusComment, a.DateLastModified, po.FrameCode, fc.FrameNotes, ins.FrameDescription,
	fc.FrameCode, ins.FrameCode, Lab.SiteCode, Lab.SiteName, fc.IsFOC, ins.IsInsert
) orders
where ClinicSiteCode = @SiteCode
	and (@Status is Null or orders.statuscode = @Status)
	and (@Priority is null or orders.orderpriority = @Priority)
group by  orders.LabCode,orders.LabName, statuscode,orderpriority
END
GO
/****** Object:  StoredProcedure [dbo].[GetTurnAroundTime_Days]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  25 Apr 16 - baf - Added Status 16 - Clinic Checkin NO Lab Checkout
--		8 Nov 16 - baf - Modified code to look at flag for IsFOC or IsInsert
-- =============================================
CREATE PROCEDURE [dbo].[GetTurnAroundTime_Days]
	-- Add the parameters for the stored procedure here
	@FromDate DateTime,
	@ToDate DateTime,
	@SiteCode VarChar(6) = Null,
	@Status VarChar(2) = Null,
	@Priority VarChar(1) = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

Select LabCode, SiteName, Avg(AvgFOC) as FOC_A, Max(MaxFOC) as FOC_Max, Min(MinFOC) as FOC_Min,
	Avg(AvgNoTintIns) as Avg_WO, Max(MaxNoTintIns) as Max_WO, Min(MinNoTintIns) as Min_WO,
	Avg(AvgTint) as Avg_T, Max(MaxTint) as Max_T, Min(MinTint) as Min_T,
	Avg(AvgIns) as Avg_I, Max(MaxIns) as Max_I, Min(MinIns) as Min_I
From
(
	Select z.DateRec, z.DateSent, z.LabCode, z.SiteName,AVG(FOCDays) as AvgFOC, MAX(FOCDays) as MaxFOC, MIN(FOCDays) as MinFOC,
		Case when (NoInsDays > 0 and NoTintDays > 0) then AVG(NoInsDays) else 0 end AvgNoTintIns,
		case when (NoInsDays > 0 and NoTintDays > 0) then MAX(NoInsDays) else 0 end MaxNoTintIns,
		Case when (NoInsDays > 0 and NoTintDays > 0) then MIN(NoInsDays) else 0 end MinNoTintIns,
		case when (NoInsDays > 0 and TintDays > 0) then AVG(TintDays) else 0 end AvgTint,
		case when (NoInsDays > 0 and TintDays > 0) then MAX(TintDays) else 0 end MaxTint,
		case when (NoInsDays > 0 and TintDays > 0) then MIN(TintDays) else 0 end MinTint,
		case when InsDays > 0 then AVG(InsDays) else 0 end AvgIns,
		case when InsDays > 0 then MAX(InsDays) else 0 end MaxIns,
		case when InsDays > 0 then MIN(InsDays) else 0 end MinIns
	from
	(
		Select OS.OrderNumber, OS.FrameCode, OS.DateLastModified as DateSent, OrdRec.DateLastModified as DateRec, os.LabCode,sc.SiteName,
			case when fc.IsFOC = 0 then 0 else DateDiff(d,OS.DateLastModified, OrdRec.DateLastModified) end FOCDays,
			case when ins.IsInsert = 0 then 0 else DATEDIFF(d,OS.DateLastModified, OrdRec.DateLastModified) end InsDays,
			Case when Ins.IsInsert = 0 then DATEDIFF(d,OS.DateLastModified, OrdRec.DateLastModified) end NoInsDays,
			case when OS.Tint <> 'CL' then DATEDIFF(d,OS.DateLastModified, OrdRec.DateLastModified) end TintDays,
			Case when OS.Tint = 'CL' then DATEDIFF(d,OS.DateLastModified, OrdRec.DateLastModified) end NoTintDays,
			Avg(DATEDIFF(d,OS.DateLastModified, OrdRec.DateLastModified)) as TotDays_A,
			Max(DateDiff(d,OS.DateLastModified, Ordrec.DateLastModified)) as TotDays_Max,
			Min(DateDiff(d,OS.DateLastModified, OrdRec.DateLastModified)) as TotDays_Min
			
		from
		(
			select SUBSTRING(po.Demographic, 5,2) AS statuscode,ClinicSiteCode, po.LabSiteCode as LabCode, Tint, LensType, LensMaterial, po.FrameCode,
				po.OrderNumber, a.OrderStatusTypeID,a.StatusComment, a.DateLastModified,
					REPLACE(SUBSTRING(po.Demographic, 1, 3), '*', '') AS rankcode,
					SUBSTRING(po.Demographic, 8, 1) AS orderpriority,
					SUBSTRING(po.Demographic, 4, 1) AS bos
			from dbo.PatientOrder po right join 
			(select OrderNumber, OrderStatusTypeID, StatusComment, LabSiteCode, IsActive, DateLastModified
			from dbo.PatientOrderStatus
			where OrderStatusTypeID in (1,2,6) 
			and DateLastModified between @FromDate and DATEADD(d,+1,@ToDate)) a 
			on a.OrderNumber = po.OrderNumber
			where
			ClinicSiteCode = @SiteCode
			and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status,SUBSTRING(po.Demographic,5,2))
			and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority,Substring(po.demographic,8,1))
		) OS
		left join
		(
			select SUBSTRING(po.Demographic, 5,2) AS statuscode,ClinicSiteCode, po.LabSiteCode as LabCode,  Tint, LensType, LensMaterial, po.FrameCode,
				po.OrderNumber, a.OrderStatusTypeID,a.StatusComment, a.DateLastModified, COUNT(a.OrderNumber) as TotNbrRec,
				COUNT(po.FrameCode) as NbrFrame, 
				REPLACE(SUBSTRING(po.Demographic, 1, 3), '*', '') AS rankcode,
					SUBSTRING(po.Demographic, 8, 1) AS orderpriority,
					SUBSTRING(po.Demographic, 4, 1) AS bos
			from dbo.PatientOrder po right join 
			(select OrderNumber, OrderStatusTypeID, StatusComment, LabSiteCode, IsActive, DateLastModified
			from dbo.PatientOrderStatus
			where OrderStatusTypeID in (11,16) and
			DateLastModified between @FromDate and @ToDate) a -- Change Dates to parameters
			on a.OrderNumber = po.OrderNumber
			where ClinicSiteCode = @SiteCode
			and SUBSTRING(po.Demographic,5,2) = ISNULL(@Status,SUBSTRING(po.Demographic,5,2))
			and SUBSTRING(po.Demographic,8,1) = ISNULL(@Priority,Substring(po.demographic,8,1))
			Group by Demographic, ClinicSiteCode, po.LabSiteCode, a.LabSiteCode,Tint, LensMaterial,LensType,po.OrderNumber,
			a.OrderStatusTypeID, a.StatusComment, a.DateLastModified, po.FrameCode
		) OrdRec on OS.OrderNumber = OrdRec.OrderNumber
		left join
			--(Select * from dbo.Frame where FrameNotes like '%FOC%') fc on OS.FrameCode = fc.FrameCode
			(Select * from dbo.Frame where IsFOC = 1 and IsActive = 1) fc on OS.FrameCode = fc.FrameCode
			left join 
			--(Select * from dbo.Frame where FrameDescription like '%INSERT%') ins on OS.FrameCode = ins.FrameCode
			(Select * from dbo.Frame where IsInsert = 1 and IsActive = 1) ins on OS.FrameCode = ins.FrameCode
			left join 
			(Select SiteName, SiteCode from dbo.SiteCode) sc on OS.LabCode = sc.SiteCode
		Where OS.DateLastModified < OrdRec.DateLastModified
		Group by OS.LabCode, SiteName, OS.OrderNumber, OS.FrameCode, OS.DateLastModified, OrdRec.DateLastModified, fc.FrameNotes,
			ins.FrameDescription, OS.Tint, fc.IsFOC, ins.IsInsert
	) z
	Group by LabCode, SiteName, NoInsDays, NoTintDays, TintDays, InsDays, DateSent, DateRec, OrderNumber
) RptDays
Group by LabCode, SiteName--, TotDays_A, TotDays_Max, TotDays_Min
order by LabCode

END
GO
/****** Object:  StoredProcedure [dbo].[GetStandardFrames]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 9/26/2013
-- Description:	This stored procedure retrieves all data necessary
--		to complete the Standard Frame portion of the 
--		Summary for All Orders Processed (Rpt 54).
-- =============================================
CREATE PROCEDURE [dbo].[GetStandardFrames]
	-- Add the parameters for the stored procedure here
	@FromDate Datetime,
	@ToDate Datetime,
	@LabSiteCode varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select LabSiteCode, FrameCode, SUM(NAD) as TNAD, SUM(NRet) as TNRET, 
	SUM(MAD) as TMAD, SUM(MRet) as TMRet, SUM(AAD) as TAAD, SUM(ARet) as TARet,
	SUM(FAD) as TFAD, sum(FRet) as TFRet, SUM(PDRL) as TPDRL, SUM(FmrPOW) as TPOW,
	SUM(CAD) as TCAD, SUM(PAD) as TPAD, SUM(BAD) as TBAD, SUM(FRes) as TFRES, 
	SUM(ARes) as TARes, SUM(ANG) as TANG, sum(FNG) as TFNG,SUM(NRes) as TNRes,
	SUM(NDepAD) as TNDep, SUM(MRes) as TMRes, SUM(DodRemCONUS) as TDRCONUS, 
	SUM(VABen) as TVABen, SUM(IMET) as TIMET, SUM(Other) as TOther
from
(
	select LabSiteCode, FrameCode,
		case when a.bos = 'N' and statuscode in (11,14) then Pairs else 0 end NAD,
		case when a.bos = 'N' and statuscode = 31 then Pairs else 0 end NRet,
		case when a.bos = 'M' and statuscode = 11 then Pairs else 0 end MAD,
		case when a.bos = 'M' and statuscode = 31 then Pairs else 0 end MRet,
		case when a.bos = 'A' and statuscode in (11,14) then Pairs else 0 end AAD,
		case when a.bos = 'A' and statuscode = 31 then Pairs else 0 end ARet,
		case when a.bos = 'F' and statuscode in (11,14) then Pairs else 0 end FAD,
		case when a.bos = 'F' and statuscode = 31 then Pairs else 0 end FRet,
		case when statuscode = 32 then Pairs else 0 end PDRL,
		case when statuscode = 36 then Pairs else 0 end FmrPOW,
		case when a.bos = 'C' and statuscode = 11 then Pairs else 0 end CAD,
		case when a.bos = 'P' and statuscode = 11 then Pairs else 0 end PAD,
		case when a.bos = 'B' and statuscode = 11 then Pairs else 0 end BAD,
		case when a.bos = 'F' and statuscode = 12 then Pairs else 0 end FRes,
		case when a.bos = 'A' and statuscode = 12 then Pairs else 0 end ARes,
		case when a.bos = 'A' and statuscode = 15 then Pairs else 0 end ANG,
		case when a.bos = 'F' and statuscode = 15 then Pairs else 0 end FNG,
		case when a.bos = 'N' and statuscode = 12 then Pairs else 0 end NRes,
		case when a.bos = 'N' and statuscode = 41 then Pairs else 0 end NDepAD,
		case when a.bos = 'M' and statuscode = 12 then Pairs else 0 end MRes,
		case when a.bos = 'K' and statuscode = 55 then Pairs else 0 end DodRemCONUS,
		case when a.bos = 'K' and statuscode = 61 then Pairs else 0 end VABen,
		case when a.bos = 'K' and statuscode = 71 then Pairs else 0 end IMET,
		case when statuscode not in (11,12,15,31,41,12,55,61,71, 32,36) then Pairs else 0 end Other
	from
	(
		select pos.OrderStatusTypeID, po.ClinicSiteCode, po.LabSiteCode,
			REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
			SUBSTRING(i.Demographic, 5,2) AS statuscode,
			SUBSTRING(i.Demographic, 8, 1) AS orderpriority,
			SUBSTRING(i.Demographic, 4, 1) AS bos, po.FrameCode, po.LensType,
			po.Pairs, f.FrameDescription, f.FrameNotes,
			case when f.FrameNotes like '%FOC%' THEN 1 ELSE 0 END IsFOC,
			F.IsInsert, IsMultivision
		from dbo.PatientOrder po left join dbo.PatientOrderStatus pos
		on po.OrderNumber = pos.OrderNumber
		inner join Individual i on i.ID = po.IndividualID_Patient
		inner join Frame f on f.FrameCode = po.FrameCode
		where poS.DateLastModified between @FromDate and @ToDate and 
		pos.OrderStatusTypeID > 2 and pos.OrderStatusTypeID not in (3,4,5)
		and FrameNotes not in  ('JUSTIFY', 'FOC') AND 
		IsInsert = 0  and pos.IsActive = 1
	) a
)b
where LabSiteCode = @LabSiteCode
Group by LabSiteCode, FrameCode
order by LabSiteCode, FrameCode
END
GO
/****** Object:  StoredProcedure [dbo].[GetSRTSOrdersDetailRpt]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 16 October 2013
-- Description:	This stored procedure retrieves the data
--		required to produce the SRTS Orders Detail Report
-- Modified:  1/5/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetSRTSOrdersDetailRpt]
	-- Add the parameters for the stored procedure here
	@MySiteCode VarChar(6),
	@LabSiteCode VarChar(6),
	@FromDate Datetime,
	@ToDate DATETIME,
	@ModifiedBy	VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--------------------- Audit MultiRead 10/27/14 - BAF
	DECLARE @ReadDate		DATETIME = GETDATE()
	DECLARE @PatientList	VARCHAR(MAX)
	DECLARE @Notes			VARCHAR(MAX)
	
	SELECT @PatientList = COALESCE(@PatientList, 'Patient IDs: ') + Cast(po.IndividualID_Patient AS VARCHAR(20)) + ', '
	FROM dbo.PatientOrder po left join dbo.Prescription pr on po.PrescriptionID = pr.ID
		INNER join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
		inner join dbo.Individual ind on ind.ID = pr.IndividualID_Patient
		inner join dbo.IndividualIDNumber id on ind.ID = id.IndividualID
		inner join dbo.SiteAddress sa on sa.SiteCode = po.LabSiteCode 
	where pos.OrderStatusTypeID in (1,6) and OnholdForConfirmation = 0
		 and pos.DateLastModified between @FromDate and @ToDate
		 and sa.AddressType = 'SITE'
		 and po.LabSiteCode = @LabSiteCode and po.ClinicSiteCode = @MySiteCode
		 
	SELECT @Notes = COALESCE(@Notes, 'Order Numbers: ') + po.OrderNumber + ', '
	FROM dbo.PatientOrder po left join dbo.Prescription pr on po.PrescriptionID = pr.ID
		inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
		inner join dbo.Individual ind on ind.ID = pr.IndividualID_Patient
		inner join dbo.IndividualIDNumber id on ind.ID = id.IndividualID
		inner join dbo.SiteAddress sa on sa.SiteCode = po.LabSiteCode 
	where pos.OrderStatusTypeID in (1,6) and OnholdForConfirmation = 0
		 and pos.DateLastModified between @FromDate and @ToDate
		 and sa.AddressType = 'SITE'
		 and po.LabSiteCode = @LabSiteCode and po.ClinicSiteCode = @MySiteCode
		 
	IF @Notes IS NOT NULL
		EXEC dbo.InsertAuditMultiRead @ReadDate, @ModifiedBy, 12, @PatientList, @Notes
		
	----------------------------------------------------

select distinct po.LabSiteCode, po.ClinicSiteCode, ind.LastName, ind.FirstName, substring(ind.MiddleName,1,1) MI, 
	right(id.IDNumber,4) as Last4,ODSphere, ODCylinder, ODAxis, ODAdd,ODSegHeight, 
	OSSphere, OSCylinder, OSAxis, OSAdd,pr.PDNear, pr.PDDistant, pr.ODPDDistant, pr.ODPDNear, 
	pr.OSPDDistant, pr.OSPDNear, OSSegHeight,po.OrderNumber,
	po.FrameCode, po.FrameColor, po.FrameEyeSize, po.FrameTempleType, po.LensType,
	po.Tint, po.Coatings, po.LensMaterial, po.Pairs, po.NumberOfCases, -- Added Coatings 1/5/18
	pos.DateLastModified as DtOrdered, sa.Address1, sa.Address2, sa.Address3, sa.City, sa.[State],
	sa.ZipCode, sa.Country, po.IndividualID_Patient
from
dbo.PatientOrder po left join dbo.Prescription pr on po.PrescriptionID = pr.ID
inner join dbo.PatientOrderStatus pos on pos.OrderNumber = po.OrderNumber
inner join dbo.Individual ind on ind.ID = pr.IndividualID_Patient
inner join dbo.IndividualIDNumber id on ind.ID = id.IndividualID
inner join dbo.SiteAddress sa on sa.SiteCode = po.LabSiteCode 
where pos.OrderStatusTypeID in (1,6) and OnholdForConfirmation = 0
 and pos.DateLastModified between @FromDate and @ToDate
 and sa.AddressType = 'SITE' AND id.IDNumberType = 'SSN'
 and po.LabSiteCode = @LabSiteCode and po.ClinicSiteCode = @MySiteCode
order by po.LabSiteCode, po.IndividualID_Patient
END
GO
/****** Object:  StoredProcedure [dbo].[GetReimburseablesReport]    Script Date: 10/03/2019 13:59:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 27 Feb 2014
-- Description:	This stored procedure will pull all 
--		reimburseable orders for a specified time period
--		and either all labs or a specific lab
-- Modified: 1/5/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[GetReimburseablesReport]
	-- Add the parameters for the stored procedure here
	@SiteCode VarChar(6)=null,
	@FromDate Datetime,
	@ToDate Datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select po.OrderNumber,pos.StatusComment, pos.OrderStatusTypeID, po.IndividualID_Patient, po.FrameCode,
	po.FrameBridgeSize, po.FrameColor, po.FrameEyeSize, po.FrameTempleType,
	po.LensMaterial, po.LensType, po.Tint,  po.Coatings, po.LabSiteCode, -- added Coatings 1/5/18
	REPLACE(SUBSTRING(i.Demographic, 1, 3), '*', '') AS rankcode,
	SUBSTRING(i.Demographic, 4,2) AS statuscode,
	SUBSTRING(i.Demographic, 7, 1) AS orderpriority,
	SUBSTRING(i.Demographic, 6, 1) AS bos, po.ShipToPatient,
	fit.Text as TintText, fic.Text as ColorText,
	CASE WHEN po.Coatings = 'AR' THEN 'Anti-Refl'
		WHEN po.Coatings = 'UV' THEN 'UV-400'
		WHEN po.coatings = 'AR/UV' THEN 'Anti-Refl/UV-400'
		WHEN po.Coatings = 'UV/AR' THEN 'UV-400/Anti-Refl'
	END CoatingsText,
	fitt.Text as TempleText, f.FrameDescription as FrameDesc,
	fib.Text as BridgeText, fie.Text as EyeText,
	lub.Text as BOSDesc, lus.Text as StatusDesc, 
	film.Text as LensMat, filt.Text as LensTypeDesc,
	luop.Text as OrderPriDesc,
	sac.SiteName as ClinicName,
	sal.SiteName as LabName,
	pos.datelastmodified as DispensedDate,
	Case when SUBSTRING(i.Demographic,6,1) = 'C' and SUBSTRING(i.Demographic,4,2) = 11 then 1 else 0 end CGAD, -- Coast Guard Active Duty
	Case when SUBSTRING(i.Demographic,6,1) = 'B' and SUBSTRING(i.Demographic,4,2) = 11 then 1 else 0 end NOAAAD, -- NOAA Active Duty
	Case when SUBSTRING(i.Demographic,6,1) = 'P' and SUBSTRING(i.Demographic,4,2) = 11 then 1 else 0 end PHSAD, -- Public Health Service Active Duty
	Case when SUBSTRING(i.Demographic,6,1) = 'C' and SUBSTRING(i.Demographic,4,2) = 12 then 1 else 0 end CGRes, -- Coast Guard Reserve
	Case when SUBSTRING(i.Demographic,6,1) = 'B' and SUBSTRING(i.Demographic,4,2) = 12 then 1 else 0 end NOAARes, -- NOAA Reserve
	Case when SUBSTRING(i.Demographic,6,1) = 'P' and SUBSTRING(i.Demographic,4,2) = 12 then 1 else 0 end PHSRes, -- Public Health Service Reserve
	Case when SUBSTRING(i.Demographic,6,1) = 'C' and SUBSTRING(i.Demographic,4,2) = 31 then 1 else 0 end CGRet, -- Coast Guard Retired
	Case when SUBSTRING(i.Demographic,6,1) = 'B' and SUBSTRING(i.Demographic,4,2) = 31 then 1 else 0 end NOAARet, -- NOAA Retired
	Case when SUBSTRING(i.Demographic,6,1) = 'P' and SUBSTRING(i.Demographic,4,2) = 31 then 1 else 0 end PHSRet, -- Public Health Service Retired
	Case when SUBSTRING(i.Demographic,4,2) = 57 then 1 else 0 end DODOccHealth, -- DOD Occupational Health
	Case when SUBSTRING(i.Demographic,4,2) in (41,43) then 1 else 0 end DEP, -- Active Duty and Retiree Dependents
	Case when SUBSTRING(i.demographic,4,2) = 55 then 1 else 0 end DODRemoteEmpConus, -- DOD Employee - Remote CONUS
	Case when SUBSTRING(i.Demographic,4,2) = 56 then 1 else 0 end DODRemoteDepConus, -- DOD Dependent - Remote CONUS
	Case when SUBSTRING(i.Demographic,4,2) = 76 then 1 else 0 end FNCiv, -- Foreign National Civilian
	Case when SUBSTRING(i.Demographic,4,2) = 74 then 1 else 0 end FNNonNato, -- Foreign National NonNato
	Case when SUBSTRING(i.Demographic,4,2) = 71 then 1 else 0 end IMETSales, -- IMET and Foreign Military Sales Trainees
	Case when SUBSTRING(i.Demographic,4,2) = 61 then 1 else 0 end VABene, -- VA Beneficiary
	Case when SUBSTRING(i.Demographic,4,2) = 67 then 1 else 0 end Ethnic, -- American Indian, Aleut, Eskimo
	Case when SUBSTRING(i.Demographic,4,2) = 52 then 1 else 0 end DeptState,  -- Department of State Dependent - Overseas
	Case when SUBSTRING(i.Demographic,4,2) = 51 then 1 else 0 end DeptStateEmp,  -- Department of State Employee - Oversees
	Case when SUBSTRING(i.Demographic,4,2) = 59 then 1 else 0 end OtherEmp, -- Other Employee & Dependents (Red Cross, USO, etc)
	Case when SUBSTRING(i.Demographic,4,2) = 65 then 1 else 0 end Contr,	-- Contractors 
	Case when SUBSTRING(i.Demographic,4,2) = 53 then 1 else 0 end OtrFedEmp, -- Other Federal Agency Employee
	Case when SUBSTRING(i.Demographic,4,2) = 54 then 1 else 0 end OtrFedDep, -- Other Federal Agency Dependent
	case when substring(i.Demographic,4,2) = 12 then 1 else 0 end Res, -- Reserves
	case when SUBSTRING(i.demographic,4,2) = 15 then 1 else 0 end NB -- National Guard
from dbo.PatientOrder po 
	left join dbo.PatientOrderStatus pos on po.OrderNumber = pos.OrderNumber
	inner join Individual i on i.ID = po.IndividualID_Patient
	inner join Frame f on f.FrameCode = po.FrameCode
	inner join FrameItem fit on fit.Value = po.Tint
	inner join FrameItem fic on fic.Value = po.FrameColor
	inner join FrameItem fitt on fitt.Value = po.FrameTempleType
	inner join FrameItem fib on fib.Value = po.FrameBridgeSize
	inner join FrameItem fie on fie.Value = po.FrameEyeSize
	inner join LookupTable lub on lub.value = SUBSTRING(i.Demographic,6,1)
	inner join LookupTable lus on lus.value = SUBSTRING(i.Demographic,4,2)
	inner join FrameItem filt on filt.Value = po.LensType
	inner join FrameItem film on film.Value = po.LensMaterial
	inner join LookupTable luop on luop.Value = SUBSTRING(i.Demographic,7,1)
	inner join SiteCode sal on sal.SiteCode = po.LabSiteCode
	inner join SiteCode sac on sac.SiteCode = po.ClinicSiteCode
where lub.Code = 'BOSType' and lus.Code = 'PatientStatusType'
	and ((OrderStatusTypeID in (7,19) and ShipToPatient = 1) or
		(OrderStatusTypeID = 8 and ShipToPatient = 0))
	and pos.DateLastModified between @FromDate and DATEADD(d,+1,@ToDate) 
	and (@SiteCode is null or po.LabSiteCode = @SiteCode)
	and SUBSTRING(i.Demographic,4,2) not in (11,14,21,31)
Order by LabSiteCode, bos, statuscode,  rankcode desc, orderpriority,pos.DateLastModified, po.OrderNumber
END
GO
/****** Object:  StoredProcedure [dbo].[InsertOrderEmail]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 19 March 2018
-- Description:	Used to enter a record in OrderEmail
-- =============================================
CREATE PROCEDURE [dbo].[InsertOrderEmail]
	@OrderNumber VARCHAR(16),
	@EmailAddress VARCHAR(500),
	@EmailMsg VARCHAR(MAX) = '',
	@EmailDateTime DATETIME = Null,
	@PatientID INT,
	@ChangeEmail BIT,
	@EmailSent BIT = 0,
	@Success INT OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID INT
	
	SELECT @ErrorLogID = 0,
		@Success = 0
	
	BEGIN TRY
		BEGIN TRANSACTION
			
			INSERT INTO dbo.OrderEmail
			VALUES (@OrderNumber, @EmailAddress, @EmailMsg, @EmailDateTime, @PatientID, @ChangeEmail, @EmailSent)
			
			If @ChangeEmail = 1
			BEGIN
			-- Change Individual Email Address
				Update dbo.IndividualEmailAddress
				Set EmailAddress = @EmailAddress
				where IndividualID = @PatientID
			END

		COMMIT TRANSACTION
		SET @Success = 1
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
		SET @Success = 0
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH		

END
GO
/****** Object:  StoredProcedure [dbo].[InsertNewIndividual]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================`
 Author:		Barb Fieldhausen
 Create date: 	2/25/2016
 Description:	This is a major modification to the original
		InsertNewIndividual due to multiple IndividualTypes being
		Added at one time, removed extra selects
 =============================================*/
/*NOTE: This Stored Procedure affects Insert New Individual, Insert New Patient and BMT Uploads */
CREATE PROCEDURE [dbo].[InsertNewIndividual] 
@IDNumber				varchar(15),
@IDNumberType			varchar(3),
@PersonalType			varchar(25),
@UserID					UniqueIdentifier = Null,
@FirstName				varchar(75),
@LastName				varchar(75),
@MiddleName				varchar(75),
@DateOfBirth			DATETIME = '01/01/1900',
@EADStopDate			datetime = '01/01/1900',
@IsPOC					BIT = 0,
@SiteCodeID				varchar(6),
@Comments				varchar(256) = '',
@IsActive				BIT = 1,
@LocationCode			varchar(10) = '',
@Demographic			VARCHAR(8),
@ModifiedBy				varchar(200),
@NextFOCDate			DATETIME = NULL,
@ROBAcceptance			DateTime = Null

AS


BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET ANSI_NULLS ON;
	
	DECLARE @ErrorLogID int,
			@TempID int = Null,
			@TempLName varchar(75),
			@TypeID INT,
			@RecCnt INT,
			@IndividualID INT
			
	SELECT @TempID = IndividualID FROM dbo.IndividualIDNumber 
	WHERE IDNumber = @IDNumber AND LOWER(IDNumberType) = LOWER(@IDNumberType)
	
	CREATE TABLE #IDs (RowID INT, ID VARCHAR(3))

	BEGIN 
		IF LEN(@PersonalType) > 4
			BEGIN
				
				INSERT INTO #IDs  (RowID, ID)
				SELECT * FROM dbo.SplitStrings(@PersonalType,',')	
				
				SELECT @RecCnt = MAX(RowID) FROM #IDs;
			END
		ELSE
			BEGIN
				SET @RecCnt = 1
			END
	END
	
	--Is the identification number presented on record? YES - Return record, it may be a duplicate ID
	IF(@TempID IS NOT NULL) 
		BEGIN 
		-- Individual record exists in the database
	
		IF @RecCnt = 0 
			BEGIN
				EXEC InsertIndividualType @TempID, @PersonalType, 1, 0, @ModifiedBy, 0
			END	
		END
	ELSE 
	BEGIN -- Individual record does NOT exist in the database
			BEGIN TRY
				BEGIN TRANSACTION
					IF LEN(@Demographic) < 8 and (LEFT(@Demographic,3) = 'K00' or LEFT(@Demographic,3) = 'K65') 
						Begin
							Set @Demographic = 'CIV' + @Demographic
						End					
					
					INSERT INTO dbo.Individual
					(--PersonalType,
					    aspnet_UserID, FirstName, MiddleName, LastName, DateOfBirth, Demographic, 
						EADStopDate, IsPOC, SiteCodeID, Comments, IsActive, TheaterLocationCode, 
						ModifiedBy, DateLastModified, LegacyID,NextFOCDate, ROBAcceptance
					)
					VALUES
					(--@PersonalType, 
						@UserID, @FirstName, @MiddleName, @LastName, @DateOfBirth, @Demographic, 
						@EADStopDate, @IsPOC, @SiteCodeID, @Comments, @IsActive, @LocationCode, 
						@ModifiedBy, GETDATE(), '0', @NextFOCDate, @ROBAcceptance
					)
				    
					SELECT @TempID = @@IDENTITY
					
					Exec dbo.InsertIndividualType @TempID, @PersonalType, 1, 0, @ModifiedBy, 0
					
					EXEC dbo.InsertIndividualIDNumber @TempID, @IDNumber, @IDNumberType, 1, @ModifiedBy
				COMMIT TRANSACTION
			END TRY  
			BEGIN CATCH
			
				IF XACT_STATE() <> 0 BEGIN
					EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT
					ROLLBACK TRANSACTION;
				END
				
			END CATCH	
	END

	--Auditing Routines Added 10/7/14 - BAF
	Declare @DateLastModified DateTime = GetDate()
	Declare @RecID VarChar(50) = Cast(@TempID as VarChar(50))
	Declare @IndividualInfo VarChar(Max) = (ISNULL(@PersonalType,'') + ', ' + @FirstName + ', ' + @MiddleName
		 + ', ' + @LastName + ', ' + ISNULL(CAST(@DateOfBirth as VarChar(20)),'') + ', ' + @Demographic + ', ' + 
		 Cast(@EADStopDate as VarChar(20)) + ', ' + Cast(@IsPOC as Varchar(1)) + ', ' + @SiteCodeID + ', ' + 
		 @Comments + ', ' + Cast(@IsActive as VarChar(1)) + ', ' + 
		 ISNULL(@LocationCode,'') + ', ' + @ModifiedBy + ', ' + Cast(@DateLastModified as VarChar(20)) + ', 0'
		)
		 
	If @IndividualInfo is not null
		EXEC InsertAuditTrail @DateLastModified,'ALL Individual',@IndividualInfo,@RecID, 5, @ModifiedBy, 'I'
	
	------------------------------
	SELECT a.Comments, a.DateLastModified, a.DateOfBirth, a.Demographic, 
		a.EADStopDate, a.FirstName, a.ID, a.IsActive, a.IsPOC, a.LastName, 
		a.TheaterLocationCode, a.MiddleName, a.ModifiedBy, lt.Value as PersonalType, 
		a.SiteCodeID, a.Comments, b.IDNumber, b.IDNumberType, a.NextFOCDate
	FROM dbo.Individual a
		INNER JOIN dbo.IndividualIDNumber b ON a.ID = b.IndividualID
		INNER JOIN dbo.IndividualType it ON a.ID = it.IndividualID
		INNER JOIN dbo.LookupTable lt ON it.TypeID = lt.ID				
	WHERE a.ID = @TempID
END
GO
/****** Object:  StoredProcedure [dbo].[InsertPatientOrderStatus]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
 Author:		<Author,,Name>
 Create date: <Create Date,,>
 Description:	<Description,,>
 Modified:  3/31/2015 - baf - To modify OrderNumber to
		VarChar (Max) to allow multiple order numbers to be
		passed in one call, parse and loop through
	1/29/2016 - baf - Added If statement to update
		PatientOrder when Lab Redirects the order
	2/10/2016 - baf - Added check for comment, will pull
		Status comment of no user supplied comment.
	3/29/2016 - baf - Added Success Parameter and commented out the
		OrderReturn....no longer needed by the code
	1/24/2017 - baf - Modified FOC to use PrevFOCDate for FOC Orders.	
	2/15/17 - baf - Modified to repull patient demographic information
		when existing order status is 15 (Incomplete) and new status is not 14 (Cancelled)
		Bug-093
	2/16/17 - baf - Added test to check for current status 16 (Clinic Force Check-in) and
		new status not 8 (Clinic Dispensed) or 17 (Reclaimed Order)...new status will be
		bypassed if status is 16 and 8 or 17...if false then new status will be updated.
	8/18/17 - baf - Removed status 17 from new status - being used as Return To Stock now.
	7/24/18 – baf – Modified to ensure only lab that checked in an order can check it out
	8/29/18 - baf - Modified to ensure lab check out does not occur after a lab reject.
 =============================================*/
CREATE PROCEDURE [dbo].[InsertPatientOrderStatus] 
@OrderNumber		VARCHAR(MAX),--VARCHAR(16),
@OrderStatusTypeID	INT,
@StatusComment		VARCHAR(500),
@LabSiteCode		VARCHAR(6),
@IsActive			BIT,
@ModifiedBy			VARCHAR(200),
@LegacyID			INT = 0,
@Success			INT OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID	INT = 0,
		@RecCnt INT,
		@Order VARCHAR(16),
		@CurRec INT = 1,
		@IsFOC bit = 0,
		@FrameCode VarChar(5),
		@DispCnt INT,
		@CurStatus INT,
		@PatientID INT,
		@MaxDate AS DATETIME,
		@ShipAddress1 VARCHAR(200)= '',
		@ShipAddress2 VARCHAR(200) = '',
		@ShipAddress3 VARCHAR(200) = '',
		@ShipCity VARCHAR(200) = '',
		@ShipState VARCHAR(2) = '',
		@ShipCountry VARCHAR(2) = '',
		@ShipZipCode VARCHAR(10) = '',
		@PatientPhoneID INT,
		@EmailADdress VARCHAR(200),
		@DateLastModified DateTime,
		@CurLab varchar(6)

		
	BEGIN TRY
	
		Select @DateLastModified = GETDATE()
		DECLARE @ForceChkIn INT,
			@WrongLab Bit
	
		-- Need to loop through the order numbers if there is more than one
		-- Added 3/31/15 -- Check-In/Check-Out Changes
		
		CREATE TABLE #tmpOrderNbrs
			(RowID int, OrderNbr VarChar(16))
			
		INSERT INTO #tmpOrderNbrs
		SELECT * FROM dbo.SplitStrings(@OrderNumber,',')	

		SELECT * FROM #tmpOrderNbrs;
		
		Select @RecCnt = MAX(RowID) FROM #tmpOrderNbrs;
		SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
			
		while @RecCnt >= @CurRec 
		Begin
			BEGIN TRANSACTION
			SET NOCOUNT OFF;

				-- Get Current OrderStatus -- 2/15/17
				Select @CurStatus = OrderStatusTypeID, @CurLab = LabSiteCode FROM dbo.PatientOrderStatus 
					WHERE OrderNumber = @Order AND IsActive = 1
					
				SELECT @ForceChkIn = COUNT(*) FROM dbo.PatientOrderStatus 
				WHERE ORderNumber = @Order AND OrderStatusTypeID = 16				

				-- Added 2/10/15 - baf Check for comment
				If @StatusComment is null or @StatusComment = '' 
				Begin
					Select @StatusComment = OrderStatusDescription from dbo.OrderStatusType where ID = @OrderStatusTypeID
				End
				
				If (@CurStatus is null or @CurStatus not in (8,19)) --and @OrderStatusTypeID not in (17)
				Begin

				-- Get PatientID and repull demographic info - 2/15/17
				
				SELECT @PatientID = IndividualID_Patient FROM dbo.PatientOrder WHERE
					OrderNumber = @Order
					
				IF @CurStatus = 15 AND @OrderStatusTypeID <> 14 
					-- if this is an incomplete order, and new status is not cancelled, ensure you have the 
					-- lastest demographic info for the patient in the order
				BEGIN
					SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualAddress WHERE IndividualID = @PatientID
					SET @ShipAddress1 = (SELECT Address1 FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SET @ShipAddress2 = (SELECT Address2 FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SET @ShipAddress3 = (SELECT Address3 FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SET @ShipCity = (SELECT City FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SET @ShipState = (SELECT [State] FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SET @ShipCountry = (SELECT Country FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SET @ShipZipCode = (SELECT ZipCode FROM dbo.IndividualAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = @MaxDate)
					SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualPhoneNumber WHERE IndividualID = @PatientID
					SET @PatientPhoneID = (SELECT ID FROM dbo.IndividualPhoneNumber WHERE IndividualID = @PatientID AND IsActive = 1)-- AND DateLastModified = @MaxDate)
					SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualEMailAddress WHERE IndividualID = @PatientID
					SET @EmailADdress = (SELECT EmailAddress FROM dbo.IndividualEMailAddress WHERE IndividualID = @PatientID AND IsActive = 1 AND DateLastModified = 

@MaxDate)
					
					IF @ShipAddress1 IS NULL
					BEGIN
						UPDATE dbo.PatientOrder SET PatientEmail = @EmailADdress, PatientPhoneID = @PatientID
						WHERE  OrderNumber = @Order AND IndividualID_Patient = @PatientID
					END
					ELSE
					BEGIN
						UPDATE dbo.PatientOrder SET ShipAddress1 = @ShipAddress1, ShipAddress2 = @ShipAddress2, ShipAddress3 = @ShipAddress3,
							ShipCity = @ShipCity, ShipState = @ShipState, ShipCountry = @ShipCountry, ShipZipCode = @ShipZipCode,
							PatientEmail = @EmailADdress, PatientPhoneID = @PatientPhoneID
						WHERE OrderNumber = @Order AND IndividualID_Patient = @PatientID
					END
				END		
				
				--Prior to insert, set current active record to inactive.
				If @OrderStatusTypeID <> 7
				Begin
					Update dbo.PatientOrderStatus
					Set IsActive = 0
					Where OrderNumber = @Order and
					IsActive = 1
				End
				Select @FrameCode = FrameCode from dbo.PatientOrder where OrderNumber = @Order
				Select @IsFOC = IsFOC FROM dbo.Frame where FrameCode = @FrameCode
				
		/* Insert new FOCDate*/
				DECLARE @Patient INT,
					@NextFOCDate DATETIME,
					@Priority CHAR(1),
					@PrevFOCDate DATE				
				
				SELECT @Patient = IndividualID_Patient, @Priority = RIGHT(Demographic,1) 
					FROM dbo.PatientOrder
					WHERE OrderNumber = @Order
				
				SELECT @NextFOCDate = NextFOCDate, @PrevFOCDate = PrevFOCDate FROM dbo.Individual 
					WHERE ID = @Patient
				
				IF @OrderStatusTypeID in (1,3,9) AND @Priority in ('F','W') and @IsFOC = 1
				BEGIN
				--	IF (@NextFOCDate IS NULL) OR (Cast(@NextFOCDate as Date) <= Cast(GETDATE() as Date))
				--	BEGIN
						IF @NextFOCDate IS NOT NULL
						BEGIN
							UPDATE dbo.Individual SET PrevFOCDate = @NextFOCDate WHERE ID = @Patient
						End
						UPDATE dbo.Individual 
						SET NextFOCDate = DATEADD(dd,365,GETDATE())
						WHERE ID = @Patient
						--EXECUTE UpdateNextFOCDate @Patient
				--	End
				END
				ELSE 
				IF @OrderStatusTypeID IN (14,17) AND @Priority in ('F','W') and @IsFOC = 1
				BEGIN
					-- First get the Date order was Created then compare with NextFOCDate
					-- in the individual record, if it matches the calculated value, then set it to NULL, 
					-- if it doesn't match, then do nothing.  Because this record did not create the FOC Date.
					DECLARE @CreateDateFOC DATETIME
					
					SELECT @CreateDateFOC = DATEADD(dd,365,DateLastModified) FROM dbo.PatientOrderStatus
					WHERE OrderNumber = @Order AND OrderStatusTypeID IN (1,3,9) --AND StatusComment LIKE '%Created%'
					
					IF Cast(@CreateDateFOC as DATE) = Cast(@NextFOCDate as Date)
					BEGIN
						UPDATE dbo.Individual SET NextFOCDate = @PrevFOCDate, PrevFOCDate = Null
						WHERE ID = @Patient
					END
				END
				---------- END Insert FOCDate
				--Check for Prior Dispense Record
				Select @DispCnt = COUNT(OrderStatusTypeID) from dbo.PatientOrderStatus
						Where OrderNumber = @Order and OrderStatusTypeID in (7,16)
				
					If @DispCnt = 0 and @OrderStatusTypeID = 7
					Begin
						Update dbo.PatientOrderStatus
						Set IsActive = 0
						Where OrderNumber = @Order and
						IsActive = 1
					End
					
				IF @ForceChkIn = 1 AND @OrderStatusTypeID = 7
				BEGIN
					INSERT INTO dbo.PatientOrderStatus (OrderNumber, OrderStatusTypeID, StatusComment,
						LabSiteCode, IsActive, ModifiedBy, DateLastModified, LegacyID)
					VALUES (@Order, @OrderStatusTypeID, @StatusComment,@LabSiteCode,0,@ModifiedBy,
						GETDATE(),0)
				End
				ELSE 
				IF @OrderStatusTypeID = 7 AND @LabSiteCode <> @CurLab 
				BEGIN
					-- Skip the update and reset the IsActive Status Bit to one
					DECLARE @MaxStatDate DATETIME 
					
					SET @WrongLab = 1
					SElect @MaxStatDate = MAX(DateLastModified) FROM dbo.PatientOrderStatus 
					WHERE OrderNumber = @Order
				
					UPDATE dbo.PatientOrderStatus SET IsActive = 1 WHERE	
						OrderNumber= @Order AND DateLastModified = @MaxStatDate--@OrderStatusTypeID = @CurStatus
				END
				ELSE
				IF @CurStatus = 3 AND @OrderStatusTypeID = 7  -- Added 8/29/18
					BEGIN			
						SElect @MaxStatDate = MAX(DateLastModified) FROM dbo.PatientOrderStatus 
						WHERE OrderNumber = @Order
				
						UPDATE dbo.PatientOrderStatus SET IsActive = 1 WHERE	
							OrderNumber= @Order AND DateLastModified = @MaxStatDate--@OrderStatusTypeID = @CurStatus
					End
				ELSE
					IF((@DispCnt >= 1 and @OrderStatusTypeID <> 7) or (@DispCnt = 0))
					-- IF the record has already been dispensed from the lab, skip the insert
					-- OR if the lab has been force checked in at the clinic do not insert a lab check-out
					BEGIN
						Declare @ClinicShip Bit = 0
					
						Select @ClinicShip = ClinicShipToPatient from dbo.PatientOrder Where OrderNumber = @Order
						If @OrderStatusTypeID = 8 and @ClinicShip = 1 
							BEGIN
								Set @StatusComment = 'Clinic Dispense - Ship To Patient' 
							END
						
						INSERT INTO dbo.PatientOrderStatus
						( OrderNumber ,
						  OrderStatusTypeID ,
						  StatusComment ,
						  LabSiteCode , 
						  IsActive ,
						  ModifiedBy ,
						  DateLastModified, 
						  LegacyID
						)
						VALUES  ( @Order, -- OrderNumber - varchar(16)
						  @OrderStatusTypeID , -- OrderStatusTypeID - int
						  @StatusComment ,
						  @LabSiteCode , 
						  1 ,
						  @ModifiedBy , -- ModifiedBy - varchar(200)
						  @DateLastModified, --GETDATE(),    -- DateLastModified - datetime
						  @LegacyID
						)
					END 
				
			-- Update order to new lab if order is redirected.
		    IF @OrderStatusTypeID = 4
		    Begin
				UPDATE dbo.PatientOrder
				SET LabSiteCode = @LabSiteCode
				WHERE OrderNumber = @Order;
			End
			END
		    SET NOCOUNT ON;
		    
			COMMIT TRANSACTION
			Set @DispCnt = 0
			SET @Success = 1
			SET @CurRec = @CurRec + 1;
			SELECT @Order = OrderNbr FROM #tmpOrderNbrs WHERE RowID = @CurRec;
		print @RecCnt
		print @CurRec
		Print @Order
		END
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			-- Insert Failed
			SET @Success = 0 
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH
	
	--select ordernumber, orderstatustypeid, statuscomment, labsitecode, isactive, modifiedby, datelastmodified
	--from dbo.patientorderstatus
	--where ordernumber IN (SELECT OrderNbr FROM #tmpOrderNbrs)
	--and isactive = 1
	--and orderstatustypeid = @orderstatustypeid
	
	DROP TABLE #tmpOrderNbrs;
END
GO
/****** Object:  StoredProcedure [dbo].[ReprintReturn]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 15 March 2017
-- Description:	This stored procedure provides the returned reprint list
-- Modified:  7/13/2017 - baf - Added Country Name
--			7/5/17 - baf - Added ability to pull multiple statuses
--			7/19/17 - baf - Added code to calc row and column IDs
-- =============================================
CREATE PROCEDURE [dbo].[ReprintReturn]
	-- Add the parameters for the stored procedure here
	@SiteCode VARCHAR(6),
	@OrderSTatusTypeID VarChar(5),
	@DateTime SMALLDATETIME,
	@Columns int = Null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare
 	@LenStatus int, @STatus1 int = 0, @STatus2 int = 0,
 	@AlphaSort bit = 0
	
	
	Select @AlphaSort = AlphaSort from dbo.SitePref_General 
	where SiteCode = @SiteCode
	
	
	Set @LenStatus = LEN(@OrderStatusTypeID)
	if @LenStatus <= 2
		Set @STatus1 = @OrderStatusTypeID
	else if @LenStatus = 5
		BEGIN
			Set @STatus1 = Cast(LEFT(@OrderStatusTypeID,2) as Int)
			Set @STatus2 = Cast(RIGHT(@OrderSTatusTypeID,2) as Int)
		END
	else if @LenStatus = 4
		Begin
			Set @STatus1 = Cast(LEFT(@OrderStatusTypeID,1) as Int)
			Set @STatus2 = Cast(RIGHT(@OrderSTatusTypeID,2) as Int)		
		End
	IF @ORderStatusTypeID in ('11,16','11','16','8')  -- Clinic Labels
	BEGIN

	--IF @OrderStatusTypeID IN (8,11,16) -- Clinic Labels
	IF @AlphaSort = 1 -- Sort By LastName
		Begin
		Select RowID, Case when RowID <= @Columns then 1 
			when RowID%@Columns = 0 then RowID/@Columns	else (RowID/@Columns) + 1 end RowNbr,
			Case when RowID % @Columns = 0 then @Columns else RowID % @Columns end ColNbr, c.ID, 
			FirstName + ' ' + MiddleName + ' ' + LastName as Patient, ShipAddress1, 
			ShipAddress2, ShipAddress3, ShipCity, ShipState, ShipCountry, 
			lt.[Text] as CountryName, ShipZipCode, LastName
		From 
		(	
			Select RowID = ROW_NUMBER() OVER (Order BY LastName), b.*
			From (
				Select Distinct IndividualID_Patient as ID, ShipAddress1, ShipAddress2, ShipAddress3,ShipCity,
							ShipCountry, ShipState, ShipZipCode 
				From 
				(
						Select  pos.OrderNumber, IndividualID_Patient, ShipToPatient, ClinicShipToPatient,
						pos.DateLastModified, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode
						from dbo.PatientOrderStatus pos
							Inner Join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
						where left(pos.OrderNumber,6) = @SiteCode
							and Cast(pos.DateLastModified as SmallDateTime) = @DateTime
							and OrderStatusTypeID in (@Status1,@Status2)
							and (ShipToPatient = 1 or ClinicShipToPatient = 1)
							and po.ClinicSiteCode = @SiteCode
						) a
						Left Join dbo.Individual ind on ind.ID = a.IndividualID_Patient
					) b	
					Inner Join dbo.Individual ind on ind.ID = b.ID
				) c
			Inner Join dbo.Individual ind on ind.ID = c.ID
			Inner Join dbo.LookupTable lt on lt.[Value] = c.ShipCountry
		Where lt.Code = 'CountryList'
		Order By LastName
		--Select distinct ind.ID, FirstName + ' ' + MiddleName + ' ' + LastName as Patient, 
		--	ShipAddress1, ShipAddress2, ShipAddress3, ShipCity, ShipState, ShipCountry, lt.[Text] as CountryName, 
		--	ShipZipCode, LastName
		--From
		--(
		--	Select OrderNumber from dbo.PatientOrderStatus
		--	where left(OrderNumber,6) = @SiteCode
		--		and Cast(DateLastModified as SmallDateTime) = @DateTime
		--		and OrderStatusTypeID in (@Status1,@STatus2)
		--) a
		--Inner Join dbo.PatientOrder po on a.OrderNumber = po.OrderNumber
		--Inner Join dbo.Individual ind on po.IndividualID_Patient = ind.ID
		--Left Join dbo.LookupTable lt on lt.Value = ShipCountry
		--where po.ShipToPatient = 1 or po.ClinicShipToPatient = 1
		--	and po.ClinicSiteCode = @SiteCode
		--	and lt.Code = 'CountryList'
		--Order by LastName
		END
	ELSE
		Begin -- No Sort on Labels
		
		Select RowID, Case when RowID <= @Columns then 1 
			when RowID%@Columns = 0 then RowID/@Columns	else (RowID/@Columns) + 1 end RowNbr,
			Case when RowID % @Columns = 0 then @Columns else RowID % @Columns end ColNbr, c.ID, 
			FirstName + ' ' + MiddleName + ' ' + LastName as Patient, ShipAddress1, 
			ShipAddress2, ShipAddress3, ShipCity, ShipState, ShipCountry, 
			lt.[Text] as CountryName, ShipZipCode, LastName
		From 
		(	
			Select RowID = ROW_NUMBER() OVER (Order BY ShipZipCode), b.*
			From (
				Select Distinct IndividualID_Patient as ID, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode 
				From 
				(
						Select  pos.OrderNumber, IndividualID_Patient, ShipToPatient, ClinicShipToPatient,
						pos.DateLastModified, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode
						from dbo.PatientOrderStatus pos
							Inner Join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
						where left(pos.OrderNumber,6) = @SiteCode
							and Cast(pos.DateLastModified as SmallDateTime) = @DateTime
							and OrderStatusTypeID in (@Status1,@Status2)
							and (ShipToPatient = 1 or ClinicShipToPatient = 1)
							and po.ClinicSiteCode = @SiteCode
						) a
						Left Join dbo.Individual ind on ind.ID = a.IndividualID_Patient
					) b	
					Inner Join dbo.Individual ind on ind.ID = b.ID
				) c
			Inner Join dbo.Individual ind on ind.ID = c.ID
			Inner Join dbo.LookupTable lt on lt.[Value] = c.ShipCountry
		Where lt.Code = 'CountryList'
		Order by ShipZipCode		
		
		END
	END
	
	IF @OrderSTatusTypeID in ('7,19','7','19') -- Lab Labels
	BEGIN
		If @AlphaSort = 1 -- Sort by LastName
		BEGIN

		Select RowID, Case when RowID <= @Columns then 1 
			when RowID%@Columns = 0 then RowID/@Columns	else (RowID/@Columns) + 1 end RowNbr,
			Case when RowID % @Columns = 0 then @Columns else RowID % @Columns end ColNbr, c.ID, 
			FirstName + ' ' + MiddleName + ' ' + LastName as Patient, ShipAddress1, 
			ShipAddress2, ShipAddress3, ShipCity, ShipState, ShipCountry, 
			lt.[Text] as CountryName, ShipZipCode, LastName
		From 
		(	
			Select RowID = ROW_NUMBER() OVER (Order BY LastName), b.*
			From (
				Select Distinct IndividualID_Patient as ID, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode 
				From 
				(
						Select  pos.OrderNumber, IndividualID_Patient, ShipToPatient, ClinicShipToPatient,
						pos.DateLastModified, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode
						from dbo.PatientOrderStatus pos
							Inner Join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
						where pos.LabSiteCode =  @SiteCode and (OrderStatusTypeID in (@STatus1,@Status2))
							AND Cast(pos.DateLastModified as SmallDateTime) = @DateTime
						) a
						Left Join dbo.Individual ind on ind.ID = a.IndividualID_Patient
					) b	
					Inner Join dbo.Individual ind on ind.ID = b.ID
				) c
			Inner Join dbo.Individual ind on ind.ID = c.ID
			Inner Join dbo.LookupTable lt on lt.[Value] = c.ShipCountry
		Where lt.Code = 'CountryList'		
		Order By LastName

		END
		ELSE
		BEGIN -- No Sort on Labels

		Select RowID, Case when RowID <= @Columns then 1 
			when RowID%@Columns = 0 then RowID/@Columns	else (RowID/@Columns) + 1 end RowNbr,
			Case when RowID % @Columns = 0 then @Columns else RowID % @Columns end ColNbr, c.ID, 
			FirstName + ' ' + MiddleName + ' ' + LastName as Patient, ShipAddress1, 
			ShipAddress2, ShipAddress3, ShipCity, ShipState, ShipCountry, 
			lt.[Text] as CountryName, ShipZipCode, LastName
		From 
		(	
			Select RowID = ROW_NUMBER() OVER (Order BY ShipZipCode), b.*
			From (
				Select Distinct IndividualID_Patient as ID, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode 
				From 
				(
						Select  pos.OrderNumber, IndividualID_Patient, ShipToPatient, ClinicShipToPatient,
						pos.DateLastModified, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity,
							ShipCountry, ShipState, ShipZipCode
						from dbo.PatientOrderStatus pos
							Inner Join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
						where pos.LabSiteCode =  @SiteCode and (OrderStatusTypeID in (@STatus1,@Status2))
							AND Cast(pos.DateLastModified as SmallDateTime) = @DateTime
							--and pos.ModifiedBy = @ModifiedBy
						) a
						Left Join dbo.Individual ind on ind.ID = a.IndividualID_Patient
					) b	
					Inner Join dbo.Individual ind on ind.ID = b.ID
				) c
			Inner Join dbo.Individual ind on ind.ID = c.ID
			Inner Join dbo.LookupTable lt on lt.[Value] = c.ShipCountry
		Where lt.Code = 'CountryList'	
		Order By ShipZipCode
		END
	END
	
	IF @OrderStatusTypeID = '2' -- 771 ReprintOrder List
	BEGIN
	Declare @Orders VarChar(Max)

	Select @Orders = Coalesce(@Orders, '') + a.OrderNumber + ','
	From (
		SELECT pos.OrderNumber, pos.ModifiedBy,  CAST(pos.DateLastModified AS SMALLDATETIME) AS OrderKey
			FROM dbo.PatientOrderStatus pos inner join dbo.PatientOrder po on po.OrderNumber = pos.OrderNumber
			WHERE --po.ShiptoPatient = 1 and 
				pos.LabSiteCode =  @SiteCode and (OrderStatusTypeID = 2 )
				AND Cast(pos.DateLastModified as SmallDateTime) = @DateTime
				--and pos.ModifiedBy = @ModifiedBy
	) a

	Select @Orders

	END
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderStatusRecord]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Bush
-- Create date: 19 Nov 2014
-- Description:	This SP allows a user to update a PatientOrderStatus record based on ID
-- =============================================
Create PROCEDURE [dbo].[UpdateOrderStatusRecord]
	-- Add the parameters for the stored procedure here
	@ID					INT,
	@OrderNumber		VARCHAR(16),
	@OrderStatusTypeID	INT,
	@StatusComment		VARCHAR(500),
	@LabSiteCode		VARCHAR(6),
	@IsActive			BIT,
	@ModifiedBy			VARCHAR(200),
	@LegacyID			INT = 0

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID	INT = 0;

	BEGIN TRY
		BEGIN TRANSACTION

			UPDATE PatientOrderStatus
			SET OrderNumber = @OrderNumber,
				OrderStatusTypeID = @OrderStatusTypeID,
				StatusComment = @StatusComment,
				LabSiteCode = @LabSiteCode,
				IsActive = @IsActive,
				ModifiedBy = @ModifiedBy,
				LegacyID = @LegacyID
			WHERE ID = @ID
	
		COMMIT TRANSACTION
	
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderStateToApprovedByOrderNumber]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateOrderStateToApprovedByOrderNumber]
@OrderNumber	VARCHAR(16),
@LabSiteCode	VARCHAR(6),
@ModifiedBy VARCHAR(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.PatientOrderStatus
	SET IsActive = 0, ModifiedBy=@ModifiedBy, DateLastModified=GETDATE()
	WHERE OrderNumber = @OrderNumber
	AND IsActive = 1
	
	INSERT dbo.PatientOrderStatus
	        ( OrderNumber ,
	          OrderStatusTypeID ,
	          StatusComment ,
	          LabSiteCode ,
	          IsActive ,
	          ModifiedBy ,
	          DateLastModified ,
	          LegacyID
	        )
	VALUES  ( @OrderNumber , -- OrderNumber - varchar(16)
	          1 , -- OrderStatusTypeID - int
	          'Clinic Created Order' , -- StatusComment - varchar(500)
	          @LabSiteCode , -- LabSiteCode - varchar(6)
	          1 , -- IsActive - bit
	          @ModifiedBy , -- ModifiedBy - varchar(200)
	          GETDATE() , -- DateLastModified - datetime
	          0  -- LegacyID - int
	        )
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePatientOrder_Decenter]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 3/28/2014
-- Description:	This procedure updates the Patient Order
--		with calcualted Decenter Values
-- Modified:  3/12/15 - BAF - PD Changes
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePatientOrder_Decenter]
	-- Add the parameters for the stored procedure here
	@Order  VarChar (16)--,
	--@LensType  varchar(5),  ******* Removed as part of PD Changes
	--@FrameEyeSize VarChar(5),
	--@FrameBridgeSize VarChar(12),
	--@ODPDDistant Decimal(4,2),
	--@OSPDDistant Decimal(4,2),
	--@ODPDNear Decimal (4,2),
	--@OSPDNear Decimal (4,2)
AS
BEGIN
	-- SET NOCOUNT ON 
	SET NOCOUNT ON;

	/*Calculate DECENTER Values and update Patient Order Record */
		UPdate dbo.PatientOrder set ODDistantDecenter = a.ODDDec, ODNearDecenter = a.ODNDEC,
			OSDistantDecenter = a.OSDDEC, OSNearDecenter = a.OSNDEC, OSBase = a.OSNBase,
			ODBase = a.ODNBase
		from dbo.PatientOrder po inner Join
			(Select distinct * from dbo.fnDecenterCalc (@Order))a
			on po.OrderNumber = @Order
		where po.OrderNumber = @Order


END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePatientOrderStatusByOrderNumber]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePatientOrderStatusByOrderNumber] 
@OrderNumber				VARCHAR(16),
@ModifiedBy					VARCHAR(200)
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	BEGIN TRY
	UPDATE dbo.PatientOrderStatus
	SET IsActive = 0, 
	ModifiedBy = @ModifiedBy, 
	DateLastModified = GETDATE() 
	WHERE OrderNumber = @OrderNumber	

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePatientOrderEmail]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 28 March 2018
-- Description:	Update the record for a patient order email
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePatientOrderEmail]
	-- Add the parameters for the stored procedure here
	@OrderNbr VARCHAR(16),
	@EmailAddress VARCHAR(500) = Null,
	@EMailMsg VARCHAR(MAX) = NULL,
	@EmailDate DATETIME = NULL,
	@EmailSent BIT,
	@Success INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID INT
	SELECT @ErrorLogID = 0
	
	BEGIN TRY
	BEGIN Transaction

	IF @EmailAddress IS NOT NULL
	BEGIN
		UPDATE dbo.OrderEmail SET EmailAddress = @EmailAddress WHERE OrderNumber = @OrderNbr
	END
	
	IF @EmailMsg IS NOT NULL
	BEGIN
		UPDATE dbo.OrderEmail SET EmailMsg = @EmailMsg WHERE OrderNumber = @OrderNbr	
	END
	
	IF @EmailSent = 1
	BEGIN
		Update dbo.OrderEmail set EmailSent = @EmailSent, EmailDate = @EmailDate
		WHERE OrderNumber = @OrderNbr
	END
	
	COMMIT TRANSACTION
	SET @Success = 1
	END TRY
	
	BEGIN CATCH
		IF XACT_STATE() <> 0
			BEGIN
				ROLLBACK TRANSACTION;
			END

			SET @Success = 0		
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;		
	END CATCH
	
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePatientOrderByOrderNumber]    Script Date: 10/03/2019 13:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  baf - 4/13/15 - to add check for blank
--		LinkedID
--		1/3/18 - baf - Added Coatings
--		3/19/18 - baf - Added EmailPatient
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePatientOrderByOrderNumber] 
@OrderNumber			varchar(16),
@IndividualID_Patient	int,
@IndividualID_Tech		int,
@PatientPhoneID			INT = 0,
@Demographic			varchar(8),
@LensType				varchar(5) = '',
@LensMaterial			varchar(5) = '',
@Tint					varchar(5) = '',
@Coatings				VARCHAR(40) = '', -- Added 1/3/18
@ODSegHeight			varchar(2) = '',
@OSSegHeight			varchar(2) = '',
@NumberOfCases			int,
@Pairs					int,
@PrescriptionID			int,
@FrameCode				varchar(20) = '',
@FrameColor				varchar(3) = '',
@FrameEyeSize			varchar(5) = '',
@FrameBridgeSize		varchar(12) = '',
@FrameTempleType		varchar(10) = '',
@ClinicSiteCode			varchar(6),
@LabSiteCode			varchar(6),
@ShipToPatient			bit,
@ShipAddress1			VARCHAR(200) = '',
@ShipAddress2			VARCHAR(200) = '',
@ShipAddress3			varchar(200) = '',
@ShipCity				VARCHAR(200) = '',
@ShipState				VARCHAR(2) = '',
@ShipZipCode			VARCHAR(10) = '',
@ShipCountry			VARCHAR(2) = '', 
@ShipAddressType		VARCHAR(20) = '',
@LocationCode			varchar(10) = '',
@UserComment1			varchar(90) = '',
@UserComment2			varchar(90) = '',
@IsGEyes				bit,
@IsMultivision			bit,
@PatientEmail			VARCHAR(200) = '',
@MedProsDispense		BIT = 0,
@PimrsDispense			BIT = 0,
@OnholdForConfirmation	BIT = 0,
@VerifiedBy				INT = 0, 
@IsActive				BIT, 
@ModifiedBy				varchar(200),
@ONBarCode				varbinary(MAX) = 0,
@IsComplete				BIT= 0,
--@FOCDate				DATETIME = NULL,
@LinkedID				VARCHAR(16),
@TheaterLocationCode	VARCHAR(9) = NULL,
@DoLinkedUpdate			BIT = 1,
@ClinicShipToPatient	BIT = 0,
@DispenseComments	VarChar(45),
@EmailPatient		BIT = 0,
@Success				INT OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	
	DECLARE @OrderCnt INT = 0,
		@Orders VARCHAR(MAX),
		@Pos INT,
		@Len INT,
		@RecCnt INT = 0,
		@RxCnt int
		
	/*Get all the order numbers that have this linked ID. Then Loop through the numbers.
		There is a max number of orders (3) that can be modified.
	 Added for splitting pairs  3/19/15 baf
	*/
	
	DECLARE @TempOrder AS VARCHAR(16) = ''
	IF @DoLinkedUpdate = 1  -- Added 5/27/15 baf
	Begin
		If @LinkedID <> ''
		Begin
			SELECT @OrderCnt = COUNT(OrderNumber) FROM dbo.PatientOrder WHERE LinkedID = @LinkedID;
			SELECT @Orders =  COALESCE (@Orders,'') + OrderNumber + ',' 
			FROM dbo.PatientOrder WHERE LinkedID = @LinkedID;
			
			SET @Len = LEN(@Orders)
			SET @TempOrder = LEFT(@Orders,16)
			SET @Pos = 17
		End
	End
	-- If there is no linked ids on the record set the record count to 0 and the order count to 1 so that 
	-- the update will be done for the one record.
	IF @OrderCnt = 0
	BEGIN
		SET @OrderCnt = 1;
		SET @RecCnt = 0;
		SET @TempOrder = @OrderNumber;
	END
	
	WHILE @RecCnt < @OrderCnt
	BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
		
			--- Audit Routine 10/10/14 - BAF
			Declare @OrderRec as VarChar(Max),
				@FOC VARCHAR(15)
			
			--IF @FOCDate IS NULL 
			--BEGIN
			--	SET @FOC = 'NO FOC Date'
			--END
			--ELSE
			--BEGIN
			--	SET @FOC = CAST(@FOCDate AS VARCHAR(12))
			--END
			
			Select @OrderRec = Coalesce(@OrderRec,'') + ClinicSiteCode + ', ' + CAST(IndividualID_Patient as VarChar(20)) + ', ' + 
				CAST(IndividualID_Tech as VarChar(20)) + ', ' + CAST(PatientPhoneID as VarChar(20)) + ', ' + Demographic  + ', ' + 
				LensType + ', ' + LensMaterial + ', ' + Tint + ', ' + ODSegHeight + ', ' + OSSegHeight + ', ' + 
				CAST(NumberOfCases as VarChar(4)) + ', ' + CAST(Pairs as VarChar(4)) + ', ' + CAST(PrescriptionID as VarChar(20))
				+ ', ' + FrameCode + ', ' + FrameColor + ', ' + FrameEyeSize + ', ' + FrameBridgeSize + ', ' + FrameTempleType
				+ ', ' + LabSiteCode + ', ' + CAST(ShipToPatient as Char(1)) + ', ' + ShipAddress1 + ', ' + ShipAddress2 + ', ' + 
				ShipAddress3 + ', ' + ShipCity + ', ' + ShipState + ', ' + ShipCountry + ', ' + ShipAddressType + ', ' + LocationCode
				+ ', ' + UserComment1 + ', ' + UserComment2 + ', ' + CAST(IsGEyes as Char(1)) + ', ' + CAST(IsMultivision as Char(1))
				+ ', ' + CAST(OSDistantDecenter as VarChar(7)) + ', ' + CAST(ODNearDecenter as VarChar(7)) + ', ' + CAST(OSNearDecenter as VarChar(7))
				+ ', ' + ODBase + ', ' + OSBase + ', ' + CAST(VerifiedBy as VarChar(20))  + ', ' + PatientEmail + ', ' + 
				CAST(MedProsDispense as Char(1)) + ', ' + CAST(PimrsDispense as Char(1)) + ', ' + CAST(OnholdForConfirmation as Char(1))
				+ ', ' + CAST(IsActive as Char(1)) + ', ' + ModifiedBy + ', ' + CAST(DateLastModified as VarChar(20)) + ', ' + 
				CAST(LegacyID as VarChar(20)) + ', ' +  @LinkedID  + ', ' + @TheaterLocationCode + ', ' + CAST(ClinicShipToPatient AS CHAR(1))
				+ ', ' + @Coatings + ', ' + CAST(@EmailPatient as VarChar(1))
			From dbo.PatientOrder where OrderNumber = @TempOrder --@OrderNumber  3/19/15 baf
			
			DECLARE @RecDate varchar(20) = Cast(GetDate() as VarChar(20))
		    
			IF @OrderRec is not null and @TempOrder <> ''
				EXEC InsertAuditTrail @RecDate, 'Patient Order Record', @OrderRec, @TempOrder, 12,@ModifiedBy,'U'
				--Modified above used @TempOrder instead of @OrderNumber -- 3/19/15 baf
			---------------
			
--			IF @ShipToPatient = 1 
--			BEGIN
		
		DECLARE @MaxDate DATETIME
		
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient
		SET @ShipAddress1 = (SELECT Address1 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipAddress2 = (SELECT Address2 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipAddress3 = (SELECT Address3 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipCity = (SELECT City FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipState = (SELECT [State] FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipCountry = (SELECT Country FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipZipCode = (SELECT ZipCode FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualPhoneNumber WHERE IndividualID = @IndividualID_Patient
		SET @PatientPhoneID = (SELECT ID FROM dbo.IndividualPhoneNumber WHERE IndividualID = @IndividualID_Patient AND IsActive = 1)-- AND DateLastModified = @MaxDate)
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualEMailAddress WHERE IndividualID = @IndividualID_Patient
		SET @PatientEmail = (SELECT EmailAddress FROM dbo.IndividualEMailAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		
			
			IF @ShipAddress1 IS NULL
			BEGIN
				SET @ShipAddress1 = ''
			END
			
			IF @ShipAddress2 IS NULL
			BEGIN
				SET @ShipAddress2 = ''
			END
			
			IF @ShipAddress3 IS NULL
			BEGIN
				SET @ShipAddress3 = ''
			END
			
			IF @ShipCity IS NULL
			BEGIN
				SET @ShipCity = ''
			END

			IF @ShipState IS NULL
			BEGIN
				SET @ShipState = ''
			End		
			
			IF @ShipCountry IS NULL
			BEGIN
				SET @ShipCountry = ''
			END
			
			IF @ShipZipCode IS NULL
			BEGIN
				SET @ShipZipCode = ''
			END
			
			IF @PatientPhoneID IS NULL
			BEGIN
				SET @PatientPhoneID = ''
			END
			
			IF @PatientEmail IS NULL
			BEGIN
				SET @PatientEmail = ''
			End
--			end			
			
			UPDATE dbo.PatientOrder 
			SET ClinicSiteCode = @ClinicSiteCode, 
			FrameBridgeSize = @FrameBridgeSize, 
			FrameCode = @FrameCode, 
			FrameColor = @FrameColor, 
			FrameEyeSize = @FrameEyeSize, 
			FrameTempleType = @FrameTempleType, 
			IndividualID_Patient = @IndividualID_Patient, 
			IndividualID_Tech = @IndividualID_Tech, 
			PatientPhoneID = @PatientPhoneID, 
			LabSiteCode = @LabSiteCode, 
			LensMaterial = @LensMaterial, 
			LensType = @LensType, 
			LocationCode = @LocationCode, 
			NumberOfCases = @NumberOfCases, 
			Pairs = @Pairs,
			PrescriptionID = @PrescriptionID,
			Demographic = @Demographic, 
			ShipAddress1 = @ShipAddress1,
			ShipAddress2 = @ShipAddress2,
			ShipAddress3 = @ShipAddress3, 
			ShipCity = @ShipCity,
			ShipState = @ShipState,
			ShipZipCode = @ShipZipCode, 
			ShipCountry = @ShipCountry, 
			ShipAddressType = @ShipAddressType,
			ShipToPatient = @ShipToPatient, 
			Tint = @Tint, 
			Coatings = @Coatings,	-- Added 1/3/18
			ODSegHeight = @ODSegHeight,
			OSSegHeight = @OSSegHeight,
			UserComment1 = @UserComment1, 
			UserComment2 = @UserComment2, 
			IsGEyes = @IsGEyes,
			IsMultivision = @IsMultivision,
			PatientEmail = @PatientEmail,
			MedProsDispense = @MedProsDispense,
			PimrsDispense = @PimrsDispense,
			OnholdForConfirmation = @OnholdForConfirmation,
			VerifiedBy = @VerifiedBy,
			IsActive = @IsActive, 
			ModifiedBy = @ModifiedBy, 
			DateLastModified = GETDATE(),
			--FOCDate = @FOCDate,
			LinkedID = @LinkedID,
			TheaterLocationCode = @TheaterLocationCode,
			ClinicShipToPatient = @ClinicShipToPatient,
			DispenseComments = @DispenseComments,
			EmailPatient = @EmailPatient
			WHERE OrderNumber = @TempOrder --@OrderNumber  3/19/15 baf
			
		
		-- Make sure that @ONBarCode is not null before updating the field
		IF @ONBarCode <> Null 
		Begin
			Update PatientOrder
			Set ONBarCode = @ONBarCode
			where OrderNumber = @TempOrder --@OrderNumber  3/19/15 baf
		ENd
		
		
		COMMIT TRANSACTION
		SET @Success = 1
/*
	Order is already in the database, no need to update the Order State, but
	need to include UpdatePatientOrder_Decenter so that if values changed 
	then the new Decenter values can be calculated.
*/

	BEGIN
		DECLARE @ODPDDistant AS decimal(6,2),
			@OSPDDistant AS decimal(6,2),
			@ODPDNear AS decimal(6,2),
			@OSPDNear AS decimal(6,2)
			
		SELECT @ODPDDistant = ODPDDistant, @OSPDDistant = OSPDDistant, @ODPDNear = ODPDNear, 
			@OSPDNear = OSPDNear
		FROM dbo.Prescription pr
		WHERE pr.ID = @PrescriptionID	
			
		Execute dbo.UpdatePatientOrder_Decenter @TempOrder --@OrderNumber 3/19/15 baf
			--,@LensType,@FrameEyeSize, @FrameBridgeSize,
			--@ODPDDistant, @OSPDDistant, @ODPDNear, @OSPDNear
	END
	
	IF @IsActive = 0 
		BEGIN
			SELECT @RxCnt = COUNT(*) FROM dbo.PatientOrder WHERE PrescriptionID = @PrescriptionID
			IF @RxCnt = 1
				BEGIN
					UPDATE dbo.Prescription SET IsUsed = 0 WHERE ID = @PrescriptionID
				END
			--ELSE
			--	BEGIN
			--		EXECUTE dbo.UpdatePrescriptionIsUsed @RxID = @PrescriptionID, -- int
			--			@ModifiedBy = @ModifiedBy 
			--	End
		END
	ELSE
		BEGIN		
			EXECUTE dbo.UpdatePrescriptionIsUsed @RxID = @PrescriptionID, -- int
				@ModifiedBy = @ModifiedBy -- varchar(200)
		END;
		
		--- Auditing Routine 10/14/14 - BAF
		Declare @Decenter VarChar(500)
		
		Select @Decenter = Coalesce(@Decenter,'') + ', ' + CAST(ODDistantDecenter as VarChar(7))  + ', ' + 
			CAST(OSDistantDecenter as VarChar(7)) + ', ' + CAST(ODNearDecenter as VarChar(7)) + ', ' + 
			CAST(OSNearDecenter as VarChar(7))
		from dbo.PatientOrder where OrderNumber = @TempOrder --@OrderNumber 3/19/15 baf
		
		If @TempOrder <> ''
			Exec InsertAuditTrail @RecDate, 'Patient Order Decenter', @Decenter, @TempOrder, 12,@ModifiedBy,'U'
		-- Updated above exec to use @TempOrder instead of @OrderNumber 3/19/15 baf
	
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		SET @Success = 0
		RETURN
	END CATCH

			SELECT OrderNumber,
			ClinicSiteCode, 
			FrameBridgeSize, 
			FrameCode, 
			FrameColor, 
			FrameEyeSize, 
			FrameTempleType, 
			IndividualID_Patient, 
			IndividualID_Tech, 
			LabSiteCode, 
			LensMaterial, 
			LensType, 
			LocationCode, 
			NumberOfCases, 
			Pairs,
			PrescriptionID, 
			PatientPhoneID, 
			Demographic, 
			ShipAddress1, 
			ShipAddress2, 
			ShipAddress3, 
			ShipCity, 
			ShipState, 
			ShipZipCode, 
			ShipCountry, 
			ShipAddressType, 
			ShipToPatient, 
			Tint, 
			Coatings, -- Added 1/3/18
			ODSegHeight,
			OSSegHeight,
			UserComment1, 
			UserComment2, 
			IsGEyes,
			IsMultivision,
			--IsMonoCalculation,
			--PDDistant,
			--PDNear,
			--ODPDDistant,
			--ODPDNear,
			--OSPDDistant,
			--OSPDNear,
			PatientEmail,
			MedProsDispense,
			PimrsDispense,
			OnholdForConfirmation,
			VerifiedBy, 
			IsActive, 
			ModifiedBy, 
			DateLastModified,
			ClinicShipToPatient,
			DispenseComments,
			--FOCDate
			EmailPatient
			FROM dbo.PatientOrder
			WHERE OrderNumber = @TempOrder --@OrderNumber  3/19/15 baf
			
	SET @RecCnt = @RecCnt + 1
	SET @TempOrder = SUBSTRING(@Orders,@Pos+1,16)
	SET @Pos = @Pos + 17
	END
END
GO
/****** Object:  StoredProcedure [dbo].[InsertPatientOrderIncomplete]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
 Author:		<Author,,Name>
 Create date: <Create Date,,>
 Description:	<Description,,>
 Modified: 11/6/15 baf - modified to pull patient address, phone number
			and email address from the database rather than pass it.
	1/12/17 - baf - Added ClinicShipToPatient
	2/23/17 - baf - Added DispenseComments
	1/3/18 - baf - Added Coatings
	3/19/18 - baf - Added EmailPatient
	04/26/2019 - baf - Added Check for LabSitePref and possible Lab Change - Story 389
	04/30/2019 - baf - Added GroupName - Story 236
	05/16/2019 - baf - Modified LabSitePref to include Region Lab and
		added CoastGuard check orders go to MNOST1 only.
	06/07/2019 - baf - Added PatientDirectedOrder JSPECS Lab Routing
	9/10/2019 - baf - Added LensStock check for Lab Determination
-- =============================================*/

CREATE PROCEDURE [dbo].[InsertPatientOrderIncomplete] 
@OrderNumber			VARCHAR(16),
@IndividualID_Patient	int,
@IndividualID_Tech		int,
@Demographic			varchar(8),
@LensType				varchar(5),
@LensMaterial			varchar(5),
@Tint					varchar(5),
@ODSegHeight			varchar(2) = '',
@OSSegHeight			varchar(2) = '',
@NumberOfCases			int,
@Pairs					int,
@PrescriptionID			int,
@PatientPhoneID			INT = 0, 
@FrameCode				varchar(20),
@FrameColor				varchar(3),
@FrameEyeSize			varchar(5),
@FrameBridgeSize		varchar(12),
@FrameTempleType		varchar(10),
@ClinicSiteCode			varchar(6),
@LabSiteCode			varchar(6),
@ShipToPatient			bit,
@ShipAddress1			VARCHAR(200) = '',
@ShipAddress2			VARCHAR(200) = '',
@ShipAddress3			varchar(200) = '',
@ShipCity				VARCHAR(200) = '',
@ShipState				VARCHAR(2) = '',
@ShipZipCode			VARCHAR(10) = '',
@ShipCountry			VARCHAR(2) = '',
@ShipAddressType		VARCHAR(10) = '',
@LocationCode			varchar(10) = '',
@UserComment1			varchar(90) = '',
@UserComment2			varchar(90) = '',
@IsActive				bit = 1,
@IsGEyes				bit,
@IsMultivision			BIT = 0,
@PatientEmail			VARCHAR(200) = '',
@OnholdForConfirmation	BIT = 0,
@VerifiedBy				INT  = 0,
@IsComplete				BIT = 1,
@ModifiedBy				varchar(200),
--@FOCDate				DATETIME,
@LinkedID				VARCHAR(16),
@ONBarCode				VARBINARY (MAX),
@TheaterLocationCode	VarChar(9) = NULL,
@ReOrder				BIT = 0,
@CLinicShipToPatient	BIT = 0,
@DispenseComments	VarChar(45) = Null,
@Coatings				VARCHAR(40) = '',
@EMailPatient			BIT,
@Success				INT OUTPUT,
@GroupName				VarChar(25)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--DECLARE @OrderNumber varchar(16)
	--DECLARE @NextNumber INT 
	--DECLARE @pad_char VARCHAR(10) = '00000000000'
	--DECLARE @tempNumb VARCHAR(25)
	--DECLARE @FiscalYear VARCHAR(2)
	--DECLARE @MyDate datetime
	--SELECT @MyDate = GETDATE()
	DECLARE @TempID int = 0
	DECLARE @ErrorLogID int = 0
	Declare @CSVLab VarChar(6), @CMVLab VarChar(6)
	
	DECLARE @LensCoating VARCHAR(40) = ''
	Set @LensCoating = REPLACE(@Coatings,',','/')
	
	Select @CMVLab = MultiPrimary, @CSVLab = SinglePrimary from dbo.SiteCode where SiteCode = @ClinicSiteCode


--	DECLARE @AsOf DATETIME
--	SELECT @AsOf = GETDATE()
--	DECLARE @Answer INT

--	IF(MONTH(@AsOf) > 10) 
--		SET @Answer = YEAR(@AsOf) + 1
--	ELSE
--		SET @Answer = YEAR(@AsOf)
	
--	SELECT @NextNumber = MAX(OrderCount) + 1 FROM dbo.PatientOrder
--	SELECT @tempNumb = @pad_char + CONVERT(VARCHAR(20),MAX(OrderCount) + 1) FROM dbo.PatientOrder
--	SELECT @OrderNumber = @ClinicSiteCode + SUBSTRING(@tempNumb, LEN(@tempNumb) - 6, 7) + '-' + SUBSTRING(CONVERT(VARCHAR(4), @Answer), 3, 2)

	--EXEC dbo.GetNextOrderNumberBySiteCode @ClinicSiteCode, -- varchar(6)
	--    @NextNumber output, -- int
	--    @FiscalYear output -- varchar(2)

	--SELECT @tempNumb = @pad_char + CONVERT(VARCHAR(20),@NextNumber)
	--SELECT @OrderNumber = @ClinicSiteCode + SUBSTRING(@tempNumb, LEN(@tempNumb) - 6, 7) + '-' + @FiscalYear
		
		DECLARE @MaxDate DATETIME,
			@PDO BIT = 0,
			@MinDays Int = 0,
			@Min Int = 0,
			@Lab VarChar(6),
			@IDCnt Int = 0

		-- 06/07/2019 - Check for Patient Directed Orders and Update Lab if necessary
		If @ClinicSiteCode in ('008094','009900')
		Begin
			-- JSPECS Order Check Capacity
			Select @IDCnt = Count(LabSiteCode), @PDO = PatientDirectedOrders 
			from dbo.SitePref_Lab where LabSiteCode = @LabSiteCode
			Group by PatientDirectedOrders

			Select @Min =  Min(DaysToOrder) From dbo.CurrentLabCapacity

			If @IDCnt <= 1  and @PDO = 0 -- Current lab doesnt do Patient Directed Orders, Find One
				Begin
					-- Select the Lab that does Patient Directed Orders based on LensType
					If @LensType like 'S%' 
						Begin
							Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
								Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
							where IsConus = 1 and DaysToOrder = @Min and clc.LabSiteCode like 'S%'
						END
					Else
						Begin
							Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
								Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
							where IsConus = 1 and DaysToOrder <= @Min and clc.LabSiteCode like 'M%'
						End
					IF @Lab is Null and @PDO = 0 
					-- If we couldn't find a lab to process the order, send it to NOSTRA
						Begin
							Set @LabSiteCode = 'MNOST1'
						End
					Else
						Begin
							Set @LabSiteCode = @Lab 
						End
				End
			Else -- Check DaysToOrder on this lab
				If @IDCnt = 1 and @PDO = 1
				Begin
					Select @MinDays = DaysToOrder from dbo.CurrentLabCapacity where LabSiteCode = @LabSiteCode
			
					If @MinDays > @Min -- will take longer with this lab go with less days
						Begin
							If @LensType like 'S%' 
								Begin
									Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
										Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
									where IsConus = 1 and DaysToOrder = @Min and clc.LabSiteCode like 'S%'
								END
							Else
								Begin
									Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
										Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
									where IsConus = 1 and DaysToOrder = @Min and clc.LabSiteCode like 'M%'
								End

									Set @LabSiteCode = @Lab
						End
				End
		End

		-- Get Mot Recent Patient Contact Information
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient
		SET @ShipAddress1 = (SELECT Address1 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipAddress2 = (SELECT Address2 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipAddress3 = (SELECT Address3 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipCity = (SELECT City FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipState = (SELECT [State] FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipCountry = (SELECT Country FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipZipCode = (SELECT ZipCode FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualPhoneNumber WHERE IndividualID = @IndividualID_Patient
		SET @PatientPhoneID = (SELECT ID FROM dbo.IndividualPhoneNumber WHERE IndividualID = @IndividualID_Patient AND IsActive = 1)-- AND DateLastModified = @MaxDate)
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualEMailAddress WHERE IndividualID = @IndividualID_Patient
		SET @PatientEmail = (SELECT EmailAddress FROM dbo.IndividualEMailAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		
		IF @ShipAddress1 IS NULL
		BEGIN
			SET @ShipAddress1 = ''
		END
		
		IF @ShipAddress2 IS NULL
		BEGIN
			SET @ShipAddress2 = ''
		END
		
		IF @ShipAddress3 IS NULL
		BEGIN
			SET @ShipAddress3 = ''
		END
		
		IF @ShipCity IS NULL
		BEGIN
			SET @ShipCity = ''
		END

		IF @ShipState IS NULL
		BEGIN
			SET @ShipState = ''
		End		
		
		IF @ShipCountry IS NULL
		BEGIN
			SET @ShipCountry = ''
		END
		
		IF @ShipZipCode IS NULL
		BEGIN
			SET @ShipZipCode = ''
		END
		
		IF @PatientPhoneID IS NULL
		BEGIN
			SET @PatientPhoneID = ''
		END
		
		IF @PatientEmail IS NULL
		BEGIN
			SET @PatientEmail = ''
		End

	BEGIN TRY
		BEGIN TRANSACTION
				INSERT INTO dbo.PatientOrder 
				(OrderNumber, 
				IndividualID_Patient, 
				IndividualID_Tech, 
				PatientPhoneID, 
				Demographic, 
				LensType, 
				LensMaterial, 
				Tint, 
				ODSegHeight, 
				OSSegHeight, 
				NumberOfCases, 
				Pairs, 
				PrescriptionID, 
				FrameCode, 
				FrameColor, 
				FrameEyeSize, 
				FrameBridgeSize, 
				FrameTempleType, 
				ClinicSiteCode, 
				LabSiteCode, 
				ShipToPatient, 
				ShipAddress1, 
				ShipAddress2,
				ShipAddress3, 
				ShipCity, 
				ShipState, 
				ShipZipCode, 
				ShipCountry, 
				ShipAddressType, 
				LocationCode, 
				UserComment1, 
				UserComment2, 
				IsActive, 
				IsGEyes, 
				IsMultivision, 
				PatientEmail,
				MedProsDispense,
				PimrsDispense, 
				OnholdForConfirmation, 
				VerifiedBy, 
				ModifiedBy, 
				DateLastModified,
				ODDistantDecenter,
				ODNearDecenter,
				OSDistantDecenter,
				OSNearDecenter,
				ODBase,
				OSBase,
				LegacyID,
				--FOCDate,
				LinkedID,
				ONBarCode,
				TheaterLocationCode,
				ReOrder, 
				ClinicShipToPatient,
				DispenseComments,
				Coatings,
				EmailPatient,
				GroupName) 

				VALUES(@OrderNumber, @IndividualID_Patient, @IndividualID_Tech, @PatientPhoneID, @Demographic, @LensType, @LensMaterial, 
				@Tint, @ODSegHeight, @OSSegHeight, @NumberOfCases, @Pairs, @PrescriptionID, @FrameCode, @FrameColor, @FrameEyeSize, @FrameBridgeSize, 
				@FrameTempleType, @ClinicSiteCode, @LabSiteCode, @ShipToPatient, 
				@ShipAddress1, @ShipAddress2, @ShipAddress3, @ShipCity, @ShipState, @ShipZipCode, @ShipCountry, @ShipAddressType, @LocationCode, @UserComment1, @UserComment2, 
				@IsActive, @IsGEyes, @IsMultivision,  @PatientEmail, 0, 0, 
				@OnholdForConfirmation, @VerifiedBy, @ModifiedBy, GETDATE(), 0, 0, 0, 0, '', '', 0, --@FOCDate, 
				@LinkedID, @ONBarCode, @TheaterLocationCode,@ReOrder,@CLinicShipToPatient, @DispenseComments, @LensCoating, @EMailPatient,@GroupName)
				
				
		COMMIT TRANSACTION
		SET @Success = 1
		

				/*Added check for GEyes orders to ensure proper status code inserts. BAF - 2/6/14 */
		--IF @IsGEyes = 1
		--BEGIN
		--	EXECUTE dbo.InsertPatientOrderStatus @OrderNumber,6,'GEyes Created Order', @LabSiteCode, 1, @ModifiedBy, 0,@Success
		--END
		--ELSE
		--BEGIN

		--	IF(@IsComplete = 1 )
		--	BEGIN	
		--		/* Commented out On Hold for Coast Guard 7/1/15 baf*/
		--		--IF (SUBSTRING(@Demographic, 4, 1)  = 'C')--this is on hold for caost guard
		--		--BEGIN
		--		--	EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 16, 'On Hold For Approval', @LabSiteCode, 1, @ModifiedBy		
		--		--END
		--		--ELSE
		--		--BEGIN
				
		--			EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 1, 'Clinic Order Created', @LabSiteCode, 1, @ModifiedBy,0, @Success
		--		--END
		--	END
		--	Else
		--	BEGIN -- this order is incomplete
		--		EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 15, 'Incomplete Order', @LabSiteCode, 1, @ModifiedBy, 0,@success	
		--	END
		--END
		/*Added code to insert Decenter values BAF - 3/28/14*/
		IF (@IsGEyes = 1)
		BEGIN
			SELECT @IsComplete = 1
		END
		
	
		IF(@IsComplete = 1)
		BEGIN
			DECLARE @ODPDDistant AS DECIMAL(6,2),
			@OSPDDistant AS DECIMAL (6,2),
			@ODPDNear	AS DECIMAL (6,2),
			@OSPDNear	AS DECIMAL (6,2)
			
			SELECT @ODPDDistant = ODPDDistant, @OSPDDistant = OSPDDistant, @OSPDNear = OSPDNear,
				@ODPDNear = ODPDNear
			FROM dbo.Prescription WHERE id = @PrescriptionID;
			
			Execute dbo.UpdatePatientOrder_Decenter @OrderNumber--,@LensType,@FrameEyeSize, @FrameBridgeSize, @ODPDDistant,
				--@OSPDDistant, @ODPDNear, @OSPDNear--, @ModifiedBy
		End
		
		------------------ Check for Values for lab site preferences 4/26/19 baf
	
		Declare @SVHasPref Int = 0, @MVHasPref Int = 0,
			@SVLab VarChar(6), @MVLab VarChar(6), @Region Int, @RegionLab VarChar(6)

		Select @SVLab = SinglePrimary, @MVLab = MultiPrimary, @Region = Region from dbo.SiteCode where SiteCode = @ClinicSiteCode
		Select @RegionLab = [Description] from dbo.LookupTable where Code = 'Region' and [Text] = @Region
		
		If @LabSiteCode is null or @LabSiteCode = ''
		Begin
			if @LensType like 'S%'
				Begin
					Set @LabSiteCode = @SVLab
				End
			Else
				Begin
					Set @LabSiteCode = @MVLab
				End
		End
		
		Select @SVHasPref = 1 From dbo.SitePref_Lab where LabSiteCode = @SVLab
		Select @MVHasPref = 1 From dbo.SitePref_Lab where LabSiteCode = @MVLab
		
		-- Only if there is a site Preference in the SitePref_Lab table for this lab will this piece run.
		If @SVHasPref = 1 or @MVHasPref = 1
		Begin

		Declare @GTGP INT = 0, @GTGD Int = 0, 
			@SVMaxDecentrationMinus Decimal(4,2), @SVMaxDecentrationPlus Decimal(4,2),@SVMaxPrism Decimal(4,2),
			@MVMaxDecentrationMinus Decimal(4,2), @MVMaxDecentrationPlus Decimal(4,2),@MVMaxPrism Decimal(4,2),
			@ODPDDec Decimal(4,2), @ODPNDec Decimal(4,2), @OSPDDec Decimal(4,2), @OSPNDec Decimal(4,2),
			@ODHPrism as Decimal(4,2), @OSHPrism Decimal(4,2), @ODVPrism as Decimal(4,2), @OSVPrism as Decimal(4,2)

		Select @RegionLab = [Description] from dbo.LookupTable where Code = 'Region' and [Text] = @Region
		Select @SVMaxPrism = MaxPrism, @SVMaxDecentrationMinus = MaxDecentrationMinus, @SVMaxDecentrationPlus = MaxDecentrationPlus from dbo.SitePref_Lab where LabSiteCode = @SVLab
		Select @LensType = LensType, @ODPDDec = ODDistantDecenter, @ODPNDec = ODNearDecenter, @OSPDDec = OSDistantDecenter, @OSPNDec = OSNearDecenter from dbo.PatientOrder where OrderNumber = @OrderNumber
		Select @MVMaxPrism = MaxPrism, @MVMaxDecentrationMinus = MaxDecentrationMinus, @MVMaxDecentrationPlus = MaxDecentrationPlus from dbo.SitePref_Lab where LabSiteCode = @MVLab
		Select @ODHPrism = ODHPrism, @OSHPrism = OSHPrism, @ODVPrism = ODVPrism, @OSVPrism = OSVPrism from dbo.Prescription where ID = @PrescriptionID
		--Select @ODHPrism = ODHPrism, @OSHPrism = OSHPrism from dbo.Prescription where ID = @PrescriptionID
				
		Select @GTGP = Case when @ODHPrism > @SVMaxPrism then 1 else 0 end
		Select @GTGP = Case When @GTGP = 0 and (@SVMaxPrism > @OSHPrism) then 0 else 1 end
		Select @GTGP = Case	When @GTGP = 0 and (@SVMaxPrism > @ODVPrism) then 0 else 1 end
		Select @GTGP = Case when @GTGP = 0 and (@SVMaxPrism > @OSVPrism) then 0 else 1 end
		
		Select @GTGD = Case when @SVMaxDecentrationMinus > @ODPDDec then 0 else 1 end
		Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @ODPDDec) then 0 else 1 end
		Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationMinus < @ODPNDec) then 0 else 1 end
		Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @ODPNDec) then 0 else 1 end
		Select @GTGD = Case	when @GTGD = 0 and (@SVMaxDecentrationMinus < @OSPDDec) then 0 else 1 end
		Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @OSPDDec) then 0 else 1 end
		Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationMinus < @OSPNDec) then 0 else 1 end
		Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @OSPNDec)  then 0 else 1 end
				
		If @GTGP = 1 -- Single vision lab can't fabricate, check multi-vision lab
			Begin
				Set @GTGP = 0
				Select @GTGP = Case when @ODHPrism > @MVMaxPrism then 1 else 2 end
				Select @GTGP = Case When @GTGP = 0 and (@MVMaxPrism > @OSHPrism) then 1 else 2 end
				Select @GTGP = Case	When @GTGP = 0 and (@MVMaxPrism > @ODVPrism) then 1 else 2 end
				Select @GTGP = Case when @GTGP = 0 and (@MVMaxPrism > @OSVPrism) then 1 else 2 end		
			End
		If @GTGD = 1
		Begin
			Set @GTGD = 0
			Select @GTGD = Case when @MVMaxDecentrationMinus < @ODPDDec then 1 else 2 end
			Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @ODPDDec) then 1 else 2 end
			Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationMinus < @ODPNDec) then 1 else 2 end
			Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @ODPNDec) then 1 else 2 end
			Select @GTGD = Case	when @GTGD < 2 and (@MVMaxDecentrationMinus < @OSPDDec) then 1 else 2 end
			Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @OSPDDec) then 1 else 2 end
			Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationMinus < @OSPNDec) then 1 else 2 end
			Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @OSPNDec)  then 1 else 2 end
		End
			If ((@GTGP = 1 and @GTGD = 0) or (@GTGP = 0 and @GTGD = 1) or (@GTGP = 0 and @GTGD = 0))  and @LensType Not like 'S%'
				Begin
					Set @LabSiteCode = @MVLab
				End
			ELSE if ((@GTGP = 1 and @GTGD = 0) or (@GTGP = 0 and @GTGD = 1) or (@GTGP = 0 and @GTGD = 0))  and @LensType like 'S%'
				Begin
					Set @LabSiteCode = @SVLab
				END
			Else 
			If @GTGP = 2 or @GTGD = 2-- Neither Single Vision nor Multi Vision lab can fabricate, use Regional Lab
			Begin
				Set @LabSiteCode = @RegionLab
			END
		End

	-------------------- END Lab Site Pref Check 04/26/2019 -----------------------
	------------------ Check for LensStock
		Declare @RowCnt int = 0
		Select @RowCnt = COUNT(*) from dbo.LensStock where SiteCode = @LabSiteCode and IsActive = 1 and IsStocked = 1
		
		If @RowCnt > 0
		Begin
			Create Table #LensStock (Material VarChar(5), Cylinder Decimal (6,2), MaxPlus Decimal(6,2), MaxMinus Decimal(6,2))
			Insert into #LensStock
			Select Material, Cylinder, MaxPlus, MaxMinus from dbo.LensStock where SiteCode = @LabSiteCode and IsActive = 1 and IsStocked = 1

			Declare @Cylinder Decimal (6,2),
				@MaxPlus Decimal (6,2),
				@MaxMinus Decimal (6,2),
				@ODCylinder Decimal (6,2),
				@OSCylinder Decimal (6,2),
				@ODSphere Decimal (6,2),
				@OSSphere Decimal (6,2),
				@CanDo Bit = 0,
				@Mat VarChar(6)


			Select @ODCylinder = ODCylinder, @OSCylinder = OSCylinder, @ODSphere = ODSphere, @OSSphere = OSSPhere
			from dbo.Prescription where ID = @PrescriptionID
			
			Declare Stat Cursor
			For Select Material, Cylinder, MaxPLus, MaxMinus from #LensStock

			Open Stat
			Fetch Stat into @Mat, @Cylinder, @MaxPlus, @MaxMinus
			While (@@FETCH_STATUS = 0)
			Begin
				If @Mat = @LensMaterial
				Begin
				If  (((@ODCylinder between @Cylinder and 0.00) and (@OSCylinder between @Cylinder and 0.00)) and
					((@ODSphere between @MaxMinus and  @MaxPlus) and (@OSSphere Between @MaxMinus and @MaxPlus)))
					Begin
						Set @CanDo = 1
					End
				End
				Fetch Stat into @Mat, @Cylinder, @MaxPlus, @MaxMinus
			End
			Close Stat
			DeAllocate Stat
			Drop Table #LensStock

			If @CanDo = 0 and @LabSiteCode = @SVLab
			Begin
				Set @LabSiteCode = @MVLab
			End
			If @CanDo = 0 and @LabSiteCode = @MVLab
			Begin
				Set @LabSiteCode = @RegionLab
			End	
		End
		----------------- End LensStock
	---------------- 10/24/18 Check LabSwitches table
	Declare @RxODSphere Decimal(4,2),
		@RxOSSphere Decimal(4,2),
		@RxODCylinder Decimal (4,2),
		@RxOSCylinder Decimal (4,2),
		@RxODHPrism Decimal (4,2),
		@RxOSHPrism Decimal (4,2),
		@RxODVPrism Decimal (4,2),
		@RxOSVPrism Decimal (4,2),
		@TempLab VarChar(6) = ''
		
	Set @RowCnt = 0
	Select @RowCnt = COUNT(*) from dbo.LabSwitches where StartDate <= CAST(GetDate() as DATE) and EndDate >= CAST(GetDate() as DATE)
		and OrigLab = @LabSiteCode and Material = @LensMaterial 	
	IF @RowCnt > 0
	Begin
		Create Table #LabSwitch (Start Date, EndDte Date, OLab VarChar(6), TLab VarChar(6), Material VarChar(5),  AllMulti Bit, MLens VarChar(5),
			AllSingle Bit, SLens VarChar(5), MaxSphere Decimal (4,2), MinSphere Decimal (4,2), Cylinder Decimal (4,2),
			AllTint Bit, Tint VarChar(15), AllPrism Bit, HPrism Decimal (4,2), VPrism Decimal (4,2))
			
		Select @RxODSphere = ODSphere, @RxOSSphere = OSSphere, @RxODCylinder = ODCylinder, @RxOSCylinder = OSCylinder, 
			@RxODHPrism = ODHPrism, @RxOSHPrism = OSHPrism, @RxODVPrism = ODVPrism, @RxOSVPrism = OSVPrism
		From dbo.Prescription where ID = @PrescriptionID
		
		Insert into #LabSwitch (Start, EndDte, OLab, TLab, Material, AllMulti, MLens, AllSingle, SLens, MaxSphere, MinSphere, 
			Cylinder, AllTint, Tint, AllPrism, HPrism, VPrism)
		Select StartDate, EndDate, OrigLab, TempLab, Material,AllMultiLens, MLensType, AllSingleLens, SLensType, MaxSphere, MinSphere,
			Cylinder, AllTint, Tint, AllPrism, HPrism, VPrism
		from dbo.LabSwitches where StartDate <= CAST(GetDate() as DATE) and EndDate >= CAST(GetDate() as DATE)
			and OrigLab = @LabSiteCode and Material = @LensMaterial --and (((MaxSphere >= @RxODSphere) or (MaxSphere >= @RxOSSphere)) or
			--((MinSphere <= @RxODSphere) or (MinSphere <= @RxOSSphere)) and ((Cylinder = @RxODCylinder or Cylinder = @RxOSCylinder)) 
			--or AllTint = 0 or AllPrism = 0 or HPrism <> 0.00 or VPrism <> 0.00)
		
		Select @TempLab = TLab from #LabSwitch where (AllTint = 1 or Tint = @Tint) or (AllSingle = 1 or SLens = @LensType) or
			(AllMulti = 1 or MLens = @LensType) or (((MaxSphere >= @RxODSphere) or (MaxSphere >= @RxOSSphere)) or
			((MinSphere <= @RxODSphere) or (MinSphere <= @RxOSSphere)) or ((Cylinder = @RxODCylinder or Cylinder = @RxOSCylinder)) 
			or AllPrism = 1 or HPrism <> 0.00 or VPrism <> 0.00)
		
		-- Switch the lab
		If @@RowCount > 0 and @TempLab <> '' 
		Begin
			Set @LabSiteCode = @TempLab
		End
		
		Drop Table #LabSwitch

		-- Retest for Regional Lab Use
		If @GTGP = 2 or @GTGD = 2-- Neither Single Vision nor Multi Vision lab can fabricate, use Regional Lab
		Begin
			Set @LabSiteCode = @RegionLab
		END
	End
-- Check Coast Guard Orders go to MNOST1
		If Substring(@Demographic,4,1) = 'C' 
		Begin
			Set @LabSiteCode = 'MNOST1'
		End
		
-- Check for Frame Restrictions
	Declare @FrLab VarChar(6) = ''

	Select @FrLab = SiteCode from dbo.FrameRestrictions where FrameCode = @FrameCode and SiteType = 'LAB'
	If @@ROWCOUNT <> 0 
		Begin
			Set @LabSiteCode = @FrLab
		End
-- Set the correct Lab on the order
	If @LabSiteCode is null
		Begin	
			If @LensType like 'S%'
			Begin
				Set @LabSiteCode = @SVLab
			ENd	
			Else
			Begin
				Set @LabSiteCode = @MVLab
			End
			--Set @LabSiteCode = @RegionLab		
	End
-- Set the correct Lab on the order
	Update dbo.PatientOrder set LabSiteCode = @LabSiteCode where OrderNumber = @OrderNumber

	----------- End check LabSwitches	


		/*Added Update for IsUsed on Prescription. 7/9/14 - BAF 
			Removed on 5/22/15 - BAF so that an incomplete order can be placed
			and the IsUsed flag is not being set on the prescription, this way
			the prescription can be modified if it is the only order on that prescription.
		*/
		--BEGIN
		--	EXECUTE dbo.UpdatePrescriptionIsUsed @PrescriptionID, @ModifiedBy
		--END
		
		/*Added check for GEyes orders to ensure proper status code inserts. BAF - 2/6/14 */

		IF @IsGEyes = 1
		BEGIN
			EXECUTE dbo.InsertPatientOrderStatus @OrderNumber,6,'GEyes Created Order', @LabSiteCode, 1, @ModifiedBy,0,@Success
		END
		ELSE
		BEGIN
			IF(@IsComplete = 1)
			BEGIN	
				IF (SUBSTRING(@Demographic, 4, 1)  = 'C')--this is on hold for caost guard
				BEGIN
					EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 16, 'On Hold For Approval', @LabSiteCode, 1, @ModifiedBy,0, @Success
				END
				ELSE
				BEGIN
					EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 1, 'Clinic Order Created', @LabSiteCode, 1, @ModifiedBy,0, @Success
				END
			END
			ELSE
			BEGIN -- this order is incomplete
				EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 15, 'Incomplete Order', @LabSiteCode, 1, @ModifiedBy,0, @Success
			END
		END

		--IF (@IsGEyes = 1)
		--BEGIN
		--	SELECT @IsComplete = 1
		--END
		--IF(@IsComplete = 1)
		--BEGIN
		--	DECLARE @ODPDDistant	DECIMAL(6,2),
		--		@OSPDDistant		DECIMAL(6,2),
		--		@ODPDNear			DECIMAL(6,2),
		--		@OSPDNear			DECIMAL(6,2)

		--	SELECT @ODPDDistant = ODPDDistant, @OSPDDistant = OSPDDistant, @ODPDNear = ODPDNear,
		--		@OSPDNear = @OSPDNear
		--	FROM dbo.Prescription 
		--	WHERE ID = @PrescriptionID
		
		--	Execute dbo.UpdatePatientOrder_Decenter @OrderNumber

		--End
		
		
		------ Auditing Routines Added  10/8/14 - BAF
		Declare @DateLastModified Datetime = GetDate()
		Declare @MEDPROSDispense int = 0
		Declare @PIMRSDispense int = 0
		Declare @Decenter VarChar(50),
			@FOC VARCHAR(15)
		
		Select @Decenter = Coalesce(@Decenter,'') + CAST(ODDistantDecenter as VarChar(6)) + ', ' + CAST(OSDistantDecenter as VarChar(6))
			+ ', ' + CAST(ODNearDecenter as VarChar(6)) + ', ' + CAST(OSNearDecenter as VarChar(6))
		From dbo.PatientOrder 
		where OrderNumber = @OrderNumber
		
		--IF @FOCDate IS NULL 
		--	BEGIN
		--		SET @FOC = 'NO FOC Date'
		--	END
		--ELSE
		--	BEGIN
		--		SET @FOC = CAST(@FOCDate AS VARCHAR(12))
		--	END
				
		Declare @OrderInfo VarChar(Max) = (@OrderNumber + ', ' + Cast(@IndividualID_Patient as VarChar(15)) + ', ' + Cast(@IndividualID_Tech as VarChar(15)) 
			+ ', ' + Cast(@PatientPhoneID as VarChar(15)) + ', ' + @Demographic + ', ' + @LensType + ', ' + @LensMaterial + ', ' + @Tint + ', ' + @ODSegHeight 
			+ ', ' + @OSSegHeight + ', ' + Cast(@NumberOfCases as VarChar(3)) + ', ' + Cast(@Pairs as VarChar(3)) + ', ' + Cast(@PrescriptionID as VarChar(15)) 
			+ ', ' + @FrameCode + ', ' + @FrameColor + ', ' + @FrameEyeSize + ', ' + @FrameBridgeSize + ', ' + @FrameTempleType + ', ' + @ClinicSiteCode 
			+ ', ' + @LabSiteCode + ', ' + Cast(@ShipToPatient as VarChar(1)) + ', ' + @ShipAddress1 + ', ' + @ShipAddress2 + ', ' + @ShipAddress3 + ', ' 
			+ @ShipCity + ', ' + @ShipState + ', ' + @ShipZipCode + ', ' + @ShipCountry + ', ' + @ShipAddressType + ', ' + @LocationCode + ', ' + 
			@UserComment1 + ', ' + @UserComment2 + ', ' + Cast(@IsActive as VarChar(1)) + ', ' + Cast(@IsGEyes as VarChar(1)) + ', ' + Cast(@IsMultivision as VarChar(1))
			+ ', ' +  @PatientEmail + ', ' + Cast(@MEDPROSDispense as VarChar(1)) + ', ' + Cast(@PIMRSDispense as VarChar(1)) 
			+ ', ' + Cast(@OnHoldForConfirmation as VarChar(1)) + ', ' + Cast(@VerifiedBy as VarChar(15)) + ', ' + @ModifiedBy
			  + ', ' + Cast(@DateLastModified as VarChar(20)) + ', ' + @Decenter + ', '  + @LinkedID + ', ' + @TheaterLocationCode + ', ' + CAST(@ReOrder AS VARCHAR(1))
			  + ', ' + @LensCoating + ', ' + CAST(@EMailPatient AS VARCHAR(1)) + ', ' + @GroupName)
			  
		If @OrderNumber is not null
			EXEC InsertAuditTrail @DateLastModified,'ALL Complete Order',@OrderInfo,@OrderNumber, 12, @ModifiedBy, 'I'
			
		--------------	
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END
		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		SET @Success = 0
		RETURN
	END CATCH
	
		--SELECT OrderNumber, IndividualID_Patient, IndividualID_Tech, PatientPhoneID, ClinicSiteCode, LabSiteCode, Demographic, LensType, LensMaterial, 
		--Tint, ODSegHeight, OSSegHeight, NumberOfCases, Pairs, PrescriptionID, FrameCode, FrameColor, FrameEyeSize, FrameBridgeSize, 
		--FrameTempleType, ClinicSiteCode, LabSiteCode, ShipToPatient, ShipAddress1, ShipAddress2, ShipAddress3, ShipCity, ShipState, ShipZipCode, ShipCountry, ShipAddressType, LocationCode, UserComment1, UserComment2, 
		--IsGEyes, IsMultivision, IsActive, PatientEmail, MedProsDispense, PimrsDispense, OnholdForConfirmation, VerifiedBy, ModifiedBy, 
		--DateLastModified, ONBarCode
		--FROM dbo.PatientOrder
		--WHERE OrderNumber = @OrderNumber 
		
END
GO
/****** Object:  StoredProcedure [dbo].[InsertPatientOrder]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*=============================================
 Author:		<Author,,Name>
 Create date: <Create Date,,>
 Description:	<Description,,>
 Modified:  7/1/15 baf - Commented out On Hold for Coast Guard
		7/29/15 baf - corrected Theater Location Code
	    11/6/15 baf - modified to pull patient address, phone number
			and email address from the database rather than pass it.
		06/13/16 baf - modified insert patient order status to include success parameter
		1/12/17 baf - Added ClinicShipToPatient
		2/23/17 baf - Added DispenseComments
		1/3/18 - baf - Added Coatings
		10/24/2018 - baf - Add Lab Switch changes
		04/26/2019 - baf - Added Check for LabSitePref and possible Lab Change
		04/30/2019 - baf - Added GroupName - Story 236
		05/16/2019 - baf - Modified LabSitePref to include Region Lab and
			added CoastGuard check orders go to MNOST1 only.
		06/07/2019 - baf  - Added JSPECS Patient Directed Order Lab Routing
		8/8/2019 - baf - Commented out Lab Switch and LabSitePref modules they
			moved to GetPatientOrderLab
	    9/10/2019 - baf - Added Lens Stock check for Lab Determination	
 =============================================*/

CREATE PROCEDURE [dbo].[InsertPatientOrder] 
@OrderNumber			VARCHAR(16),
@IndividualID_Patient	int,
@IndividualID_Tech		int,
@Demographic			varchar(8),
@LensType				varchar(5),
@LensMaterial			varchar(5),
@Tint					varchar(5),
@ODSegHeight			varchar(2),
@OSSegHeight			varchar(2),
@NumberOfCases			int,
@Pairs					int,
@PrescriptionID			int,
@PatientPhoneID			INT = 0, 
@FrameCode				varchar(20),
@FrameColor				varchar(3),
@FrameEyeSize			varchar(5),
@FrameBridgeSize		varchar(12),
@FrameTempleType		varchar(10),
@ClinicSiteCode			varchar(6),
@LabSiteCode			varchar(6),
@ShipToPatient			bit,
@ShipAddress1			VARCHAR(200) = '',
@ShipAddress2			VARCHAR(200) = '',
@ShipAddress3			varchar(200) = '',
@ShipCity				VARCHAR(200) = '',
@ShipState				VARCHAR(2) = '',
@ShipZipCode			VARCHAR(10) = '',
@ShipCountry			VARCHAR(2) = '',
@ShipAddressType		VARCHAR(10) = '',
@LocationCode			varchar(10),
@UserComment1			varchar(90),
@UserComment2			varchar(90),
@IsActive				bit = 1,
@IsGEyes				bit,
@IsMultivision			BIT,
@PatientEmail			VARCHAR(200) = '',
@OnholdForConfirmation	BIT = 0,
@VerifiedBy				INT,
@IsComplete				BIT = 1,
@ModifiedBy				varchar(200),
--@FOCDate				DATETIME = NULL,
@LinkedID				VARCHAR(16),
@ONBarCode				VARBINARY (MAX) = 0,
@TheaterLocationCode	VarChar(9) = '',
@ReOrder				BIT = 0,
@ClinicShipToPatient	BIT = 0,
@DispenseComments 	VarChar(45) = '',
@Coatings				VARCHAR(40) = '',
@EmailPatient			BIT = 0,
@Success				INT OUTPUT,
@GroupName				VarChar(25) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--DECLARE @NextNumber INT 
	--DECLARE @pad_char VARCHAR(10) = '00000000000'
	--DECLARE @tempNumb VARCHAR(25)
	--DECLARE @FiscalYear VARCHAR(2)
	--DECLARE @MyDate datetime
	--SELECT @MyDate = GETDATE()
	DECLARE @TempID int = 0
	DECLARE @ErrorLogID int = 0
--	Declare @CSVLab VarChar(6), @CMVLab VarChar(6)
	
	DECLARE @LensCoating VARCHAR(40) = ''
	Set @LensCoating = REPLACE(@Coatings,',','/')
	
--	Select @CMVLab = MultiPrimary, @CSVLab = SinglePrimary from dbo.SiteCode where SiteCode = @ClinicSiteCode

	IF @IsGEyes = 0 
	Begin
		DECLARE @MaxDate DATETIME,
			@PDO BIT = 0,
			@MinDays Int = 0,
			@Min Int = 0,
			@Lab VarChar(6),
			@IDCnt Int = 0

		-- 06/07/2019 - Check for Patient Directed Orders and Update Lab if necessary
		If @ClinicSiteCode in ('008094','009900')
		Begin
			-- JSPECS Order Check Capacity
			Select @IDCnt = Count(LabSiteCode), @PDO = PatientDirectedOrders 
			from dbo.SitePref_Lab where LabSiteCode = @LabSiteCode
			Group by PatientDirectedOrders

			Select @Min =  Min(DaysToOrder) From dbo.CurrentLabCapacity

			If @IDCnt <= 1  and @PDO = 0 -- Current lab doesnt do Patient Directed Orders, Find One
				Begin
					-- Select the Lab that does Patient Directed Orders based on LensType
					If @LensType like 'S%' 
						Begin
							Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
								Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
							where IsConus = 1 and DaysToOrder = @Min and clc.LabSiteCode like 'S%'
						END
					Else
						Begin
							Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
								Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
							where IsConus = 1 and DaysToOrder <= @Min and clc.LabSiteCode like 'M%'
						End
					IF @Lab is Null and @PDO = 0 
					-- If we couldn't find a lab to process the order, send it to NOSTRA
						Begin
							Set @LabSiteCode = 'MNOST1'
						End
					Else
						Begin
							Set @LabSiteCode = @Lab 
						End
				End
			Else -- Check DaysToOrder on this lab
				If @IDCnt = 1 and @PDO = 1
				Begin
					Select @MinDays = DaysToOrder from dbo.CurrentLabCapacity where LabSiteCode = @LabSiteCode
			
					If @MinDays > @Min -- will take longer with this lab go with less days
						Begin
							If @LensType like 'S%' 
								Begin
									Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
										Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
									where IsConus = 1 and DaysToOrder = @Min and clc.LabSiteCode like 'S%'
								END
							Else
								Begin
									Select @Lab = clc.LabSiteCode from dbo.CurrentLabCapacity clc
										Inner Join dbo.SiteAddress sa on clc.LabSiteCode = sa.SiteCode
									where IsConus = 1 and DaysToOrder = @Min and clc.LabSiteCode like 'M%'
								End

									Set @LabSiteCode = @Lab
						End
				End
		End
		--else
		--	IF @FrameCode like 'S%' 
		--	Begin
		--		Select @LabSiteCode = SinglePrimary from dbo.SiteCode where SiteCode = @ClinicSiteCode
		--	End
		--	else
		--	IF @FrameCode <> 'S%'
		--	Begin
		--		Select @LabSiteCode = MultiPrimary from dbo.SiteCode where SiteCode = @ClinicSiteCode
		--	End
		--End

		If @LabSiteCode is null or @LabSiteCode = ''
		Begin
			Select @LabSiteCode = SinglePrimary from dbo.SiteCode where SiteCode = @ClinicSiteCode
		End
		
		-- Get most recent Patient Contact Information	
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient
		SET @ShipAddress1 = (SELECT Address1 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipAddress2 = (SELECT Address2 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipAddress3 = (SELECT Address3 FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipCity = (SELECT City FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipState = (SELECT [State] FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipCountry = (SELECT Country FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SET @ShipZipCode = (SELECT ZipCode FROM dbo.IndividualAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualPhoneNumber WHERE IndividualID = @IndividualID_Patient
		SET @PatientPhoneID = (SELECT ID FROM dbo.IndividualPhoneNumber WHERE IndividualID = @IndividualID_Patient AND IsActive = 1)-- AND DateLastModified = @MaxDate)
		SELECT @MaxDate = MAX(DateLastModified) FROM dbo.IndividualEMailAddress WHERE IndividualID = @IndividualID_Patient
		SET @PatientEmail = (SELECT EmailAddress FROM dbo.IndividualEMailAddress WHERE IndividualID = @IndividualID_Patient AND IsActive = 1 AND DateLastModified = @MaxDate)
		
		IF @ShipAddress1 IS NULL
		BEGIN
			SET @ShipAddress1 = ''
		END
		
		IF @ShipAddress2 IS NULL
		BEGIN
			SET @ShipAddress2 = ''
		END
		
		IF @ShipAddress3 IS NULL
		BEGIN
			SET @ShipAddress3 = ''
		END
		
		IF @ShipCity IS NULL
		BEGIN
			SET @ShipCity = ''
		END

		IF @ShipState IS NULL
		BEGIN
			SET @ShipState = ''
		End		
		
		IF @ShipCountry IS NULL
		BEGIN
			SET @ShipCountry = ''
		END
		
		IF @ShipZipCode IS NULL
		BEGIN
			SET @ShipZipCode = ''
		END
		
		IF @PatientPhoneID IS NULL
		BEGIN
			SET @PatientPhoneID = ''
		END
		
		IF @PatientEmail IS NULL
		BEGIN
			SET @PatientEmail = ''
		End
	end
	

	BEGIN TRY
		BEGIN TRANSACTION
				INSERT INTO dbo.PatientOrder 
				(OrderNumber, 
				IndividualID_Patient, 
				IndividualID_Tech, 
				PatientPhoneID, 
				Demographic, 
				LensType, 
				LensMaterial, 
				Tint, 
				ODSegHeight, 
				OSSegHeight, 
				NumberOfCases, 
				Pairs, 
				PrescriptionID, 
				FrameCode, 
				FrameColor, 
				FrameEyeSize, 
				FrameBridgeSize, 
				FrameTempleType, 
				ClinicSiteCode, 
				LabSiteCode, 
				ShipToPatient, 
				ShipAddress1, 
				ShipAddress2,
				ShipAddress3, 
				ShipCity, 
				ShipState, 
				ShipZipCode, 
				ShipCountry, 
				ShipAddressType, 
				LocationCode, 
				UserComment1, 
				UserComment2, 
				IsActive, 
				IsGEyes, 
				IsMultivision, 
				PatientEmail,
				MedProsDispense,
				PimrsDispense, 
				OnholdForConfirmation, 
				VerifiedBy, 
				ModifiedBy, 
				DateLastModified,
				ODDistantDecenter,
				ODNearDecenter,
				OSDistantDecenter,
				OSNearDecenter,
				ODBase,
				OSBase,
				LegacyID,
				LinkedID,
				ONBarCode,
				TheaterLocationCode,
				ReOrder,
				ClinicShipTOPatient,
				DispenseComments,
				Coatings,
				EmailPatient,
				GroupName
				) 

				VALUES(@OrderNumber, @IndividualID_Patient, @IndividualID_Tech, @PatientPhoneID, @Demographic, @LensType, @LensMaterial, 
				@Tint, @ODSegHeight, @OSSegHeight, @NumberOfCases, @Pairs, @PrescriptionID, @FrameCode, @FrameColor, @FrameEyeSize, @FrameBridgeSize, 
				@FrameTempleType, @ClinicSiteCode, @LabSiteCode, @ShipToPatient, @ShipAddress1, @ShipAddress2, @ShipAddress3, @ShipCity, @ShipState, @ShipZipCode, @ShipCountry, 
				@ShipAddressType, @LocationCode, @UserComment1, @UserComment2, @IsActive, @IsGEyes, @IsMultivision, @PatientEmail, 0, 0, 
				@OnholdForConfirmation, @VerifiedBy, @ModifiedBy, GETDATE(), 0, 0, 0, 0, '', '', 0, @LinkedID, @ONBarCode, 
				LEFT(@ShipZipCode,5), @ReOrder,@ClinicShipToPatient, @DispenseComments, @LensCoating, @EmailPatient, @GroupName)
				
		COMMIT TRANSACTION
		SET @Success = 1
		
	
		/*Added Update for IsUsed on Prescription. 7/9/14 - BAF */
		BEGIN
			EXECUTE dbo.UpdatePrescriptionIsUsed @PrescriptionID, @ModifiedBy
		END
			
		/*Added check for GEyes orders to ensure proper status code inserts. BAF - 2/6/14 */
		--IF @IsGEyes = 1
		--BEGIN
		--	EXECUTE dbo.InsertPatientOrderStatus @OrderNumber,6,'GEyes Created Order', @LabSiteCode, 1, @ModifiedBy, 0,@Success
		--END
		--ELSE
		--BEGIN

		--	IF(@IsComplete = 1 )
		--	BEGIN	
		--		/* Commented out On Hold for Coast Guard 7/1/15 baf*/
		--		--IF (SUBSTRING(@Demographic, 4, 1)  = 'C')--this is on hold for caost guard
		--		--BEGIN
		--		--	EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 16, 'On Hold For Approval', @LabSiteCode, 1, @ModifiedBy		
		--		--END
		--		--ELSE
		--		--BEGIN
				
		--			EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 1, 'Clinic Order Created', @LabSiteCode, 1, @ModifiedBy,0, @Success
		--		--END
		--	END
		--	Else
		--	BEGIN -- this order is incomplete
		--		EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 15, 'Incomplete Order', @LabSiteCode, 1, @ModifiedBy, 0,@success	
		--	END
		--END
		/*Added code to insert Decenter values BAF - 3/28/14*/
		IF (@IsGEyes = 1)
		BEGIN
			SELECT @IsComplete = 1
		END
		
	
		IF(@IsComplete = 1)
		BEGIN
			DECLARE @ODPDDistant AS DECIMAL(6,2),
			@OSPDDistant AS DECIMAL (6,2),
			@ODPDNear	AS DECIMAL (6,2),
			@OSPDNear	AS DECIMAL (6,2)
			
			SELECT @ODPDDistant = ODPDDistant, @OSPDDistant = OSPDDistant, @OSPDNear = OSPDNear,
				@ODPDNear = ODPDNear
			FROM dbo.Prescription WHERE id = @PrescriptionID;
			
			Execute dbo.UpdatePatientOrder_Decenter @OrderNumber--,@LensType,@FrameEyeSize, @FrameBridgeSize, @ODPDDistant,
				--@OSPDDistant, @ODPDNear, @OSPDNear--, @ModifiedBy
		End
	
		------------------ Check for Values for lab site preferences 4/26/19 baf
		
		Declare @SVHasPref Int = 0, @MVHasPref Int = 0,
			@SVLab VarChar(6), @MVLab VarChar(6), @Region Int, @RegionLab VarChar(6)

		Select @SVLab = SinglePrimary, @MVLab = MultiPrimary, @Region = Region from dbo.SiteCode where SiteCode = @ClinicSiteCode
		Select @RegionLab = [Description] from dbo.LookupTable where Code = 'Region' and [Text] = @Region
		
		If @LabSiteCode is null or @LabSiteCode = ''
		Begin
			if @LensType like 'S%'
				Begin
					Set @LabSiteCode = @SVLab
				End
			Else
				Begin
					Set @LabSiteCode = @MVLab
				End
		End
		
		Select @SVHasPref = 1 From dbo.SitePref_Lab where LabSiteCode = @SVLab
		Select @MVHasPref = 1 From dbo.SitePref_Lab where LabSiteCode = @MVLab
		
		-- Only if there is a site Preference in the SitePref_Lab table for this lab will this piece run.
		If @SVHasPref = 1 or @MVHasPref = 1
		Begin

		Declare @GTGP INT = 0, @GTGD Int = 0, 
			@SVMaxDecentrationMinus Decimal(4,2), @SVMaxDecentrationPlus Decimal(4,2),@SVMaxPrism Decimal(4,2),
			@MVMaxDecentrationMinus Decimal(4,2), @MVMaxDecentrationPlus Decimal(4,2),@MVMaxPrism Decimal(4,2),
			@ODPDDec Decimal(4,2), @ODPNDec Decimal(4,2), @OSPDDec Decimal(4,2), @OSPNDec Decimal(4,2),
			@ODHPrism as Decimal(4,2), @OSHPrism Decimal(4,2), @ODVPrism as Decimal(4,2), @OSVPrism as Decimal(4,2)

		Select @RegionLab = [Description] from dbo.LookupTable where Code = 'Region' and [Text] = @Region
		Select @SVMaxPrism = MaxPrism, @SVMaxDecentrationMinus = MaxDecentrationMinus, @SVMaxDecentrationPlus = MaxDecentrationPlus from dbo.SitePref_Lab where LabSiteCode = @SVLab
		Select @LensType = LensType, @ODPDDec = ODDistantDecenter, @ODPNDec = ODNearDecenter, @OSPDDec = OSDistantDecenter, @OSPNDec = OSNearDecenter from dbo.PatientOrder where OrderNumber = @OrderNumber
		Select @MVMaxPrism = MaxPrism, @MVMaxDecentrationMinus = MaxDecentrationMinus, @MVMaxDecentrationPlus = MaxDecentrationPlus from dbo.SitePref_Lab where LabSiteCode = @MVLab
		Select @ODHPrism = ODHPrism, @OSHPrism = OSHPrism, @ODVPrism = ODVPrism, @OSVPrism = OSVPrism from dbo.Prescription where ID = @PrescriptionID
		--Select @ODHPrism = ODHPrism, @OSHPrism = OSHPrism from dbo.Prescription where ID = @PrescriptionID
				
		Select @GTGP = Case when @ODHPrism > @SVMaxPrism then 1 else 0 end
		Select @GTGP = Case When @GTGP = 0 and (@SVMaxPrism > @OSHPrism) then 0 else 1 end
		Select @GTGP = Case	When @GTGP = 0 and (@SVMaxPrism > @ODVPrism) then 0 else 1 end
		Select @GTGP = Case when @GTGP = 0 and (@SVMaxPrism > @OSVPrism) then 0 else 1 end
		
		Select @GTGD = Case when @ODPDDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
		Select @GTGD = Case when @ODPNDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
		Select @GTGD = Case when @OSPDDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
		Select @GTGD = Case when @OSPNDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end		
		
		--Select @GTGD = Case when @SVMaxDecentrationMinus > @ODPDDec then 0 else 1 end
		--Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @ODPDDec) then 0 else 1 end
		--Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationMinus < @ODPNDec) then 0 else 1 end
		--Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @ODPNDec) then 0 else 1 end
		--Select @GTGD = Case	when @GTGD = 0 and (@SVMaxDecentrationMinus < @OSPDDec) then 0 else 1 end
		--Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @OSPDDec) then 0 else 1 end
		--Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationMinus < @OSPNDec) then 0 else 1 end
		--Select @GTGD = Case when @GTGD = 0 and (@SVMaxDecentrationPlus > @OSPNDec)  then 0 else 1 end
				
		If @GTGP = 1 -- Single vision lab can't fabricate, check multi-vision lab
			Begin
				Set @GTGP = 0
				Select @GTGP = Case when @ODHPrism > @MVMaxPrism then 1 else 2 end
				Select @GTGP = Case When @GTGP = 0 and (@MVMaxPrism > @OSHPrism) then 1 else 2 end
				Select @GTGP = Case	When @GTGP = 0 and (@MVMaxPrism > @ODVPrism) then 1 else 2 end
				Select @GTGP = Case when @GTGP = 0 and (@MVMaxPrism > @OSVPrism) then 1 else 2 end		
			End
		If @GTGD = 1
		Begin
			Set @GTGD = 0
			--Select @GTGD = Case when @MVMaxDecentrationMinus < @ODPDDec then 1 else 2 end
			--Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @ODPDDec) then 1 else 2 end
			--Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationMinus < @ODPNDec) then 1 else 2 end
			--Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @ODPNDec) then 1 else 2 end
			--Select @GTGD = Case	when @GTGD < 2 and (@MVMaxDecentrationMinus < @OSPDDec) then 1 else 2 end
			--Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @OSPDDec) then 1 else 2 end
			--Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationMinus < @OSPNDec) then 1 else 2 end
			--Select @GTGD = Case when @GTGD < 2 and (@MVMaxDecentrationPlus > @OSPNDec)  then 1 else 2 end
			Select @GTGD = Case when @ODPDDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
			Select @GTGD = Case when @ODPNDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
			Select @GTGD = Case when @OSPDDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
			Select @GTGD = Case when @OSPNDec between @SVMaxDecentrationMinus and @SVMaxDecentrationPlus then 0 else 1 end
		End
			If ((@GTGP = 1 and @GTGD = 0) or (@GTGP = 0 and @GTGD = 1) or (@GTGP = 0 and @GTGD = 0))  and @LensType Not like 'S%'
				Begin
					Set @LabSiteCode = @MVLab
				End
			ELSE if ((@GTGP = 1 and @GTGD = 0) or (@GTGP = 0 and @GTGD = 1) or (@GTGP = 0 and @GTGD = 0))  and @LensType like 'S%'
				Begin
					Set @LabSiteCode = @SVLab
				END
			Else 
			If @GTGP = 2 or @GTGD = 2-- Neither Single Vision nor Multi Vision lab can fabricate, use Regional Lab
			Begin
				Set @LabSiteCode = @RegionLab
			END
		End

	-------------------- END Lab Site Pref Check 04/26/2019 -----------------------
	------------------ Check for LensStock
		Declare @RowCnt int = 0
		Select @RowCnt = COUNT(*) from dbo.LensStock where SiteCode = @LabSiteCode and IsActive = 1 and IsStocked = 1
		
		If @RowCnt > 0
		Begin
			Create Table #LensStock (Material VarChar(5), Cylinder Decimal (6,2), MaxPlus Decimal(6,2), MaxMinus Decimal(6,2))
			Insert into #LensStock
			Select Material, Cylinder, MaxPlus, MaxMinus from dbo.LensStock where SiteCode = @LabSiteCode and IsActive = 1 and IsStocked = 1

			Declare @Cylinder Decimal (6,2),
				@MaxPlus Decimal (6,2),
				@MaxMinus Decimal (6,2),
				@ODCylinder Decimal (6,2),
				@OSCylinder Decimal (6,2),
				@ODSphere Decimal (6,2),
				@OSSphere Decimal (6,2),
				@CanDo Bit = 0,
				@Mat VarChar(6)


			Select @ODCylinder = ODCylinder, @OSCylinder = OSCylinder, @ODSphere = ODSphere, @OSSphere = OSSPhere
			from dbo.Prescription where ID = @PrescriptionID
			
			Declare Stat Cursor
			For Select Material, Cylinder, MaxPLus, MaxMinus from #LensStock

			Open Stat
			Fetch Stat into @Mat, @Cylinder, @MaxPlus, @MaxMinus
			While (@@FETCH_STATUS = 0)
			Begin
				If @Mat = @LensMaterial
				Begin
				If  (((@ODCylinder between @Cylinder and 0.00) and (@OSCylinder between @Cylinder and 0.00)) and
					((@ODSphere between @MaxMinus and  @MaxPlus) and (@OSSphere Between @MaxMinus and @MaxPlus)))
					Begin
						Set @CanDo = 1
					End
				End
				Fetch Stat into @Mat, @Cylinder, @MaxPlus, @MaxMinus
			End
			Close Stat
			DeAllocate Stat
			Drop Table #LensStock

			If @CanDo = 0 and @LabSiteCode = @SVLab
			Begin
				Set @LabSiteCode = @MVLab
			End
			If @CanDo = 0 and @LabSiteCode = @MVLab
			Begin
				Set @LabSiteCode = @RegionLab
			End	
		End
		----------------- End LensStock
	---------------- 10/24/18 Check LabSwitches table
	Declare @RxODSphere Decimal(4,2),
		@RxOSSphere Decimal(4,2),
		@RxODCylinder Decimal (4,2),
		@RxOSCylinder Decimal (4,2),
		@RxODHPrism Decimal (4,2),
		@RxOSHPrism Decimal (4,2),
		@RxODVPrism Decimal (4,2),
		@RxOSVPrism Decimal (4,2),
		@TempLab VarChar(6) = ''
		
	Set @RowCnt = 0
	Select @RowCnt = COUNT(*) from dbo.LabSwitches where StartDate <= CAST(GetDate() as DATE) and EndDate >= CAST(GetDate() as DATE)
		and OrigLab = @LabSiteCode and Material = @LensMaterial 	
	IF @RowCnt > 0
	Begin
		Create Table #LabSwitch (Start Date, EndDte Date, OLab VarChar(6), TLab VarChar(6), Material VarChar(5),  AllMulti Bit, MLens VarChar(5),
			AllSingle Bit, SLens VarChar(5), MaxSphere Decimal (4,2), MinSphere Decimal (4,2), Cylinder Decimal (4,2),
			AllTint Bit, Tint VarChar(15), AllPrism Bit, HPrism Decimal (4,2), VPrism Decimal (4,2))
			
		Select @RxODSphere = ODSphere, @RxOSSphere = OSSphere, @RxODCylinder = ODCylinder, @RxOSCylinder = OSCylinder, 
			@RxODHPrism = ODHPrism, @RxOSHPrism = OSHPrism, @RxODVPrism = ODVPrism, @RxOSVPrism = OSVPrism
		From dbo.Prescription where ID = @PrescriptionID
		
		Insert into #LabSwitch (Start, EndDte, OLab, TLab, Material, AllMulti, MLens, AllSingle, SLens, MaxSphere, MinSphere, 
			Cylinder, AllTint, Tint, AllPrism, HPrism, VPrism)
		Select StartDate, EndDate, OrigLab, TempLab, Material,AllMultiLens, MLensType, AllSingleLens, SLensType, MaxSphere, MinSphere,
			Cylinder, AllTint, Tint, AllPrism, HPrism, VPrism
		from dbo.LabSwitches where StartDate <= CAST(GetDate() as DATE) and EndDate >= CAST(GetDate() as DATE)
			and OrigLab = @LabSiteCode and Material = @LensMaterial --and (((MaxSphere >= @RxODSphere) or (MaxSphere >= @RxOSSphere)) or
			--((MinSphere <= @RxODSphere) or (MinSphere <= @RxOSSphere)) and ((Cylinder = @RxODCylinder or Cylinder = @RxOSCylinder)) 
			--or AllTint = 0 or AllPrism = 0 or HPrism <> 0.00 or VPrism <> 0.00)
		
		Select @TempLab = TLab from #LabSwitch where (AllTint = 1 or Tint = @Tint) or (AllSingle = 1 or SLens = @LensType) or
			(AllMulti = 1 or MLens = @LensType) or (((MaxSphere >= @RxODSphere) or (MaxSphere >= @RxOSSphere)) or
			((MinSphere <= @RxODSphere) or (MinSphere <= @RxOSSphere)) or ((Cylinder = @RxODCylinder or Cylinder = @RxOSCylinder)) 
			or AllPrism = 1 or HPrism <> 0.00 or VPrism <> 0.00)
		
		-- Switch the lab
		If @@RowCount > 0 and @TempLab <> '' 
		Begin
			Set @LabSiteCode = @TempLab
		End
		
		Drop Table #LabSwitch

		-- Retest for Regional Lab Use
		If @GTGP = 2 or @GTGD = 2-- Neither Single Vision nor Multi Vision lab can fabricate, use Regional Lab
		Begin
			Set @LabSiteCode = @RegionLab
		END
	End
-- Check Coast Guard Orders go to MNOST1
		If Substring(@Demographic,4,1) = 'C' 
		Begin
			Set @LabSiteCode = 'MNOST1'
		End
		
-- Check for Frame Restrictions
	Declare @FrLab VarChar(6) = ''

	Select @FrLab = SiteCode from dbo.FrameRestrictions where FrameCode = @FrameCode and SiteType = 'LAB'
	If @@ROWCOUNT <> 0 
		Begin
			Set @LabSiteCode = @FrLab
		End
-- Set the correct Lab on the order
	If @LabSiteCode is null
		Begin	
			If @LensType like 'S%'
			Begin
				Set @LabSiteCode = @SVLab
			ENd	
			Else
			Begin
				Set @LabSiteCode = @MVLab
			End
			--Set @LabSiteCode = @RegionLab		
	End
	Update dbo.PatientOrder set LabSiteCode = @LabSiteCode where OrderNumber = @OrderNumber

	----------- End check LabSwitches	

		-- Modified Order Status Check on 8/14/18 to check for invalid frame/lens entries
		DECLARE @OrderStatus INT,
			@StatusComment VARCHAR(500)
			
		SELECT @OrderStatus = CASE WHEN @IsGEyes = 1 THEN 6
			WHEN LEFT(@LensType,1) IN ('B','D','Q','S','T') and
				LEFT(@LensMaterial,1) IN ('P','H','T','N','L') AND
				@Tint IN (SELECT Value FROM dbo.FrameItem WHERE TypeEntry = 'TINT') and
				@FrameCode IN (SELECT FrameCode FROM dbo.Frame WHERE IsActive = 1) AND
				@FrameColor IN (SELECT Value FROM dbo.FrameItem WHERE TypeEntry = 'COLOR') AND
				@FrameEyeSize NOT IN ('','X') AND
				@FrameBridgeSize NOT IN ('','X') AND 
				@FrameTempleType IN (SELECT Value FROM dbo.FrameItem WHERE TypeEntry = 'TEMPLE' AND IsActive = 1) 
				AND @IsComplete = 1 THEN 1
			ELSE 15
		END 
		
		SELECT @StatusComment = CASE WHEN @OrderSTatus = 6 THEN 'GEYES Created Order'
			WHEN @OrderStatus = 1 THEN 'Clinic Created Order'
			ELSE 'INCOMPLETE ORDER '
		END	
		
		EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, @OrderStatus, @StatusComment, @LabSiteCode, 1, @ModifiedBy, 0,@Success
		
		--/*Added check for GEyes orders to ensure proper status code inserts. BAF - 2/6/14 */
		----IF @IsGEyes = 1
		----BEGIN
		----	EXECUTE dbo.InsertPatientOrderStatus @OrderNumber,6,'GEyes Created Order', @LabSiteCode, 1, @ModifiedBy, 0,@Success
		----END
		----ELSE
		----BEGIN

		----	IF(@IsComplete = 1 )
		----	BEGIN	
		----		/* Commented out On Hold for Coast Guard 7/1/15 baf*/
		----		--IF (SUBSTRING(@Demographic, 4, 1)  = 'C')--this is on hold for caost guard
		----		--BEGIN
		----		--	EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 16, 'On Hold For Approval', @LabSiteCode, 1, @ModifiedBy		
		----		--END
		----		--ELSE
		----		--BEGIN
				
		----			EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 1, 'Clinic Order Created', @LabSiteCode, 1, @ModifiedBy,0, @Success
		----		--END
		----	END
		----	Else
		----	BEGIN -- this order is incomplete
		----		EXECUTE dbo.InsertPatientOrderStatus @OrderNumber, 15, 'Incomplete Order', @LabSiteCode, 1, @ModifiedBy, 0,@success	
		----	END
		----END
		--/*Added code to insert Decenter values BAF - 3/28/14*/
		--IF (@IsGEyes = 1)
		--BEGIN
		--	SELECT @IsComplete = 1
		--END
		
	
		--IF(@IsComplete = 1)
		--BEGIN
		--	DECLARE @ODPDDistant AS DECIMAL(6,2),
		--	@OSPDDistant AS DECIMAL (6,2),
		--	@ODPDNear	AS DECIMAL (6,2),
		--	@OSPDNear	AS DECIMAL (6,2)
			
		--	SELECT @ODPDDistant = ODPDDistant, @OSPDDistant = OSPDDistant, @OSPDNear = OSPDNear,
		--		@ODPDNear = ODPDNear
		--	FROM dbo.Prescription WHERE id = @PrescriptionID;
			
		--	Execute dbo.UpdatePatientOrder_Decenter @OrderNumber--,@LensType,@FrameEyeSize, @FrameBridgeSize, @ODPDDistant,
		--		--@OSPDDistant, @ODPDNear, @OSPDNear--, @ModifiedBy
		--End
		
		---- Check for Values for lab site preferences 4/26/19 baf
		--Declare @SVLab VarChar(6), @MVLab VarChar(6), @Region Int, @RegionLab VarChar(6),
		--	@SVMaxDecentrationMinus Decimal(4,2), @SVMaxDecentrationPlus Decimal(4,2),@SVMaxPrism Int,
		--	@MVMaxDecentrationMinus Decimal(4,2), @MVMaxDecentrationPlus Decimal(4,2),@MVMaxPrism Int,
		--	@ODPDDec Decimal(4,2), @ODPNDec Decimal(4,2), @OSPDDec Decimal(4,2), @OSPNDec Decimal(4,2)

		--Select
	
		SELECT OrderNumber, IndividualID_Patient, IndividualID_Tech, PatientPhoneID, ClinicSiteCode, LabSiteCode, Demographic, LensType, 
			LensMaterial, Tint, ODSegHeight, OSSegHeight, NumberOfCases, Pairs, PrescriptionID, FrameCode, FrameColor, FrameEyeSize, 
			FrameBridgeSize, FrameTempleType, ClinicSiteCode, LabSiteCode, ShipToPatient, ShipAddress1, ShipAddress2, ShipAddress3, 
			ShipCity, ShipState, ShipZipCode, ShipCountry, ShipAddressType, LocationCode, UserComment1, UserComment2, PatientEmail, 
			IsGEyes, IsMultivision, IsActive, MedProsDispense, PimrsDispense, OnholdForConfirmation, VerifiedBy, ModifiedBy, 
			DateLastModified,  LinkedID, ONBarCode, ReOrder, ClinicShipToPatient, DispenseComments, Coatings, EmailPatient,GroupName
		FROM dbo.PatientOrder
		WHERE OrderNumber = @OrderNumber 

		------ Auditing Routines Added  10/8/14 - BAF
		Declare @DateLastModified Datetime = GetDate()
		Declare @MEDPROSDispense int = 0
		Declare @PIMRSDispense int = 0
		Declare @Decenter VarChar(50)
		DECLARE @FOC VARCHAR(12) 
		
		Select @Decenter = Coalesce(@Decenter,'') + CAST(ODDistantDecenter as VarChar(6)) + ', ' + CAST(OSDistantDecenter as VarChar(6))
			+ ', ' + CAST(ODNearDecenter as VarChar(6)) + ', ' + CAST(OSNearDecenter as VarChar(6))
		From dbo.PatientOrder 
		where OrderNumber = @OrderNumber
		
			
		Declare @OrderInfo VarChar(Max) = (@OrderNumber + ', ' + Cast(@IndividualID_Patient as VarChar(15)) + ', ' + Cast(@IndividualID_Tech as VarChar(15)) 
			+ ', ' + Cast(@PatientPhoneID as VarChar(15)) + ', ' + @Demographic + ', ' + @LensType + ', ' + @LensMaterial + ', ' + @Tint + ', ' + @ODSegHeight 
			+ ', ' + @OSSegHeight + ', ' + Cast(@NumberOfCases as VarChar(3)) + ', ' + Cast(@Pairs as VarChar(3)) + ', ' + Cast(@PrescriptionID as VarChar(15)) 
			+ ', ' + @FrameCode + ', ' + @FrameColor + ', ' + @FrameEyeSize + ', ' + @FrameBridgeSize + ', ' + @FrameTempleType + ', ' + @ClinicSiteCode 
			+ ', ' + @LabSiteCode + ', ' + Cast(@ShipToPatient as VarChar(1)) + ', ' + @ShipAddress1 + ', ' + @ShipAddress2 + ', ' + @ShipAddress3 + ', ' 
			+ @ShipCity + ', ' + @ShipState + ', ' + @ShipZipCode + ', ' + @ShipCountry + ', ' + @ShipAddressType + ', ' + @LocationCode + ', ' + 
			@UserComment1 + ', ' + @UserComment2 + ', ' + Cast(@IsActive as VarChar(1)) + ', ' + Cast(@IsGEyes as VarChar(1)) + ', ' + Cast(@IsMultivision as VarChar(1))
			+ ', ' +  @PatientEmail + ', ' + Cast(@MEDPROSDispense as VarChar(1)) + ', ' + Cast(@PIMRSDispense as VarChar(1)) 
			+ ', ' + Cast(@OnHoldForConfirmation as VarChar(1)) + ', ' + Cast(@VerifiedBy as VarChar(15)) + ', ' + @ModifiedBy 
			  + ', ' + Cast(@DateLastModified as VarChar(20)) + ', ' + @Decenter + ', '  +  @LinkedID + ', ' +	LEFT(@ShipZipCode,5) + ', ' 
			 + CAST(@ReOrder AS VARCHAR(1)) + ', ' + CAST(@ClinicShipToPatient AS VARCHAR(1)) + ', ' + @DispenseComments + ', ' + @Coatings + 
			 ', ' + CAST(@EmailPatient AS VARCHAR(1)) + ', ' + @GroupName)
		
		  
		If @OrderNumber is not null
			EXEC InsertAuditTrail @DateLastModified,'ALL Complete Order',@OrderInfo,@OrderNumber, 12, @ModifiedBy, 'I'
			

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		SET @Success = 0
		RETURN
	END CATCH
		
END
GO
/****** Object:  StoredProcedure [dbo].[InsertBulkOrders]    Script Date: 10/03/2019 13:59:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Modified:  1/3/18 - baf - Added Coatings
-- =============================================
CREATE PROCEDURE [dbo].[InsertBulkOrders]
	@BranchOfService varchar(1),
	@ClinicSiteCode varchar(6),
	@DateOrderCreated datetime,
	@FrameBridgeSize int,
	@FrameCode varchar(20),
	@FrameColor varchar(3),
	@FrameEyeSize int,
	@FrameTempleType varchar(10),
	@IsMultiVision bit,
	@LensMaterial varchar(5),
	@LensType varchar(5),
	@LocationCode varchar(10),
	@NumberOfCases int,
	@NumberOfPairs int,
	@OdAdd decimal(4,2),
	@OdAxis int,
	@OdBase varchar(3),
	@OdCylinder decimal(4,2),
	@OdDistantDeCenter decimal(4,2),
	@OdHBase varchar(3),
	@OdHPrism decimal(4,2),
	@OdNearDeCenter decimal(4,2),
	@OdPdDistant decimal(4,2),
	@OdPdNear decimal(4,2),
	@OdSegHeight int,
	@OdSphere decimal(4,2),
	@OdVBase varchar(3),
	@OdVPrism decimal(4,2),
	@OrderPriority varchar(1),
	@OsAdd decimal(4,2),
	@OsAxis int,
	@OsBase varchar(3),
	@OsCylinder decimal(4,2),
	@OsDistantDeCenter decimal(4,2),
	@OsHBase varchar(3),
	@OsHPrism decimal(4,2),
	@OsNearDeCenter decimal(4,2),
	@OsPdDistant decimal(4,2),
	@OsPdNear decimal(4,2),
	@OsSegHeight int,
	@OsSphere decimal(4,2),
	@OsVBase varchar(3),
	@OsVPrism decimal(4,2),
	@PatientAreaCode varchar(3),
	@PatientDob datetime,
	@PatientEmail varchar(200),
	@PatientEmailType varchar(10),
	@PatientFirstName varchar(75),
	@PatientIdNumber varchar(15),
	@PatientIdNumberType varchar(3),
	@PatientLastName varchar(75),
	@PatientMiddleName varchar(75),
	@PatientPhoneNumber varchar(15),
	@PatientPhoneNumberType varchar(5),
	@PdDistant decimal(4,2),
	@PdNear decimal(4,2),
	@PrescriptionDate datetime,
	@ProviderId varchar(15),
	@ProviderIdType varchar(3),
	@RankCode varchar(3),
	@SexCode varchar(1),
	--@ShipAddress1 varchar(200),
	--@ShipAddress2 varchar(200),
	--@ShipAddress3 varchar(200),
	--@ShipAddressType varchar(20),
	--@ShipCity varchar(200),
	--@ShipState varchar(2),
	--@ShipToPatient bit,
	--@ShipZipCode varchar(5),
	--@ShipZipPlus varchar(4),
	@StatusCode varchar(2),
	@TechId varchar(15),
	@TechIdType varchar(3),
	@Tint varchar(15),
	@Coatings VARCHAR(40) = '', -- Added 1/3/18
	@UIC varchar(6),
	@UnitAddress1 varchar(200),
	@UnitAddress2 varchar(200),
	@UnitAddress3 varchar(200),
	@UserComment1 varchar(4096),
	@UserComment2 varchar(4096),
	@VerifiedBy int,
	
	--Exam Info
    @OdCorrectedAcuity varchar(10),
    @OdOsCorrectedAcuity varchar(10),
    @OdOsUncorrectedAcuity varchar(10),
    @OdUncorrectedAcuity varchar(10),
    @OsCorrectedAcuity varchar(10),
    @OsUncorrectedAcuity varchar(10),
    @ExamDate datetime,
    @ExaminerId varchar(15),
    @ExaminerIdType varchar(3),
    @ExamComment varchar(256),
    @LinkedID  VarChar(16)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	
	DECLARE @Demographic varchar(8)
	SET @Demographic = @RankCode + @BranchOfService + @StatusCode + @SexCode + @OrderPriority
	DECLARE @OrderNumber varchar(16)
	DECLARE @NextNumber INT 
	DECLARE @pad_char VARCHAR(10) = '0000000000'
	DECLARE @tempNumb VARCHAR(25)
	DECLARE @FiscalYear VARCHAR(2)
	DECLARE @MyDate datetime
	SELECT @MyDate = GETDATE()
	DECLARE @IndIdTech int;
	DECLARE @IndIdProvider int;
	DECLARE @IndIdExaminer int;
	DECLARE @TempID int
	SELECT @TempID = 0
	DECLARE @ErrorLogID int
	SELECT @ErrorLogID = 0
	DECLARE	@PatientPhoneId int;
	DECLARE @PrescriptionId int;
	DECLARE @IndIdPatient int;
	DECLARE @LabSiteCode varchar(6);
	DECLARE @IndAddressId int;
	DECLARE @INDPatientIDNumber int;
	Declare @IndEmailAddressID int;
	Declare @IndExamID int;
	Declare @DateLastModified date = GetDate();
	
	DECLARE @LensCoating VARCHAR(40)
	SET @LensCoating = REPLACE(@Coatings,',','/')
	
	DECLARE @cAddress1 varchar(200),
			@cAddress2 varchar(200),
			@cAddress3 varchar(200),
			@cCity varchar(200),
			@cState varchar(2),
			@cZip varchar(10),
			@cCountry varchar(2),
			@cAddressType varchar(4) = 'MAIL';
	-- Get the Clinic address
	SELECT
		@cAddress1 = s.Address1,
		@cAddress2 = s.Address2,
		@cAddress3 = s.Address3,
		@cCity = s.City,
		@cState = s.State,
		@cZip = s.ZipCode,
		@cCountry = s.Country
	FROM
		SiteAddress s
	WHERE
		SiteCode = @ClinicSiteCode
		AND s.AddressType = @cAddressType;
	
	-- Get lab site code for fort jackson
	SELECT 
		@LabSiteCode = SiteCode
	FROM 
		SiteAddress a
	where 
		a.City = 'FORT JACKSON'
		AND a.AddressType = 'SITE';
	
	-- Get Next Order Number
	EXEC dbo.GetNextOrderNumberBySiteCode	@ClinicSiteCode, -- varchar(6)
											@NextNumber output, -- int
											@FiscalYear output -- varchar(2)
	
	SELECT @tempNumb = @pad_char + CONVERT(VARCHAR(20),@NextNumber);
	SELECT @OrderNumber = @ClinicSiteCode + SUBSTRING(@tempNumb, LEN(@tempNumb) - 6, 7) + '-' + @FiscalYear;

	-- Get Tech Id
	SELECT 
		@IndIdTech = i.ID 
	FROM 
		IndividualIDNumber i 
	WHERE 
		i.IDNumber = @TechId AND 
		i.IDNumberType = @TechIdType;
		
	-- Get Provider Id
	SELECT
		@IndIdProvider = i.ID
	FROM
		dbo.IndividualIDNumber i
	WHERE
		i.IDNumber = @ProviderId
		AND i.IDNumberType = @ProviderIdType;
		
	-- Get Examiner Id
	SELECT
		@IndIdExaminer = i.ID
	FROM
		dbo.IndividualIDNumber i
	WHERE
		i.IDNumber = @ExaminerId
		AND i.IDNumberType = @ExaminerIdType;
	
/*
	-- Create fake data for testing purposes.
	SET @PatientIdNumber = LEFT(CAST(ABS(CAST(CAST(NEWID() as BINARY(10)) as int)) as varchar(max)) + '00000000',9);
	SELECT TOP 1 @IndIdProvider = ID FROM dbo.Individual i TABLESAMPLE(1) WHERE i.PersonalType = 'DOCTOR';
	SELECT TOP 1 @IndIdTech = ID FROM dbo.Individual i TABLESAMPLE(1) WHERE i.PersonalType = 'TECHNICIAN';
	SELECT TOP 1 @IndIdExaminer = ID FROM dbo.Individual i TABLESAMPLE(1) WHERE i.PersonalType = 'PROVIDER';
*/

	BEGIN TRY
		BEGIN TRANSACTION
		
		-- Insert patient info to the Individual table
		INSERT INTO dbo.Individual (FIrstName, MiddleName, LastName, DateOfBirth, Demographic, EADStopDate, IsPOC, SiteCodeID,
			Comments, IsActive, TheaterLocationCode,ModifiedBy, DateLastModified, LegacyID, aspnet_UserID, NextFOCDate, PrevFOCDate)
		VALUES
		(
			--'PATIENT',
			@PatientFirstName,
			@PatientMiddleName,
			@PatientLastName,
			@PatientDob,
			@Demographic,
			'01/01/1990',
			0,
			@ClinicSiteCode,
			'',
			1,
			'',
			'SYSTEM',
			GETDATE(),
			NULL,
			NULL,
			NULL,
			NULL
		);
		-- Get the newly created ID for that patient
		SELECT @IndIdPatient = SCOPE_IDENTITY();
		
		--Insert into Individual Type Table
		INSERT INTO dbo.IndividualType
		VALUES  ( @IndIdPatient , -- IndividualID - int
		          110 , -- TypeID - int *** From Lookup Table.
		          1 , -- IsActive - bit
		          'SYSTEM' , -- ModifiedBy - varchar(200)
		          GETDATE()  -- DateLastModified - datetime
		        );
		/*
			Auditing Added 10/5/2014 - BAF
			Insert Patient Individual Info to Audit_Trail
		*/
		Declare @RecID VarChar(20) = Cast(@IndIdPatient as VarChar(20))
		Declare @PatientInfo VarChar(Max) = ('PATIENT Info: ' + @RecID + ', ' + @PatientFirstName + ', ' + @PatientMiddleName + ', ' + 
			@PatientLastName + ', ' + Cast(@PatientDOB  as VarChar(20))+ ', ' + @Demographic)
		DECLARE @ChangeDate DATETIME = GetDate()
		
		EXEC InsertAuditTrail @ChangeDate,'ALL',@PatientInfo, @RecID,2,'BMTUpload','I'
				
		-- Insert patient info to the IndividualIdNumber table
		INSERT INTO dbo.IndividualIDNumber
		VALUES
		(
			@IndIdPatient,
			@PatientIdNumber,
			@PatientIdNumberType,
			1,
			'SYSTEM',
			GETDATE(),
			NULL
		);
		
		-- Get new created ID for the Patient IDNumber
		Select @INDPatientIDNumber = SCOPE_IDENTITY();
		
		--Auditing Insert added 10/5/2014 - BAF
		Declare @RecID2 VarChar(20) = Cast(@INDPatientIDNumber as VarChar(20))
		Declare @PatientIDNbr VarChar(MAX) =  ('PATIENT ID Nbr: ' + @RecID + ', ' + @RecID2 + Cast(@PatientIdNumber as VarChar(20)) 
			+ ', ' + @PatientIdNumberType)
		
		Exec InsertAuditTrail @ChangeDate,'ALL',@PatientIDNbr,@RecID2,2,'BMTUpload','I'
				
		-- Insert patient info to the IndividualPhoneNumber table
		INSERT INTO dbo.IndividualPhoneNumber
		VALUES
		(
			@IndIdPatient,
			@PatientPhoneNumberType,
			@PatientAreaCode,
			@PatientPhoneNumber,
			NULL,
			1,
			'SYSTEM',
			GETDATE(),
			NULL
		);
		-- Get the newly created ID for that patient
		SELECT @PatientPhoneId = SCOPE_IDENTITY();
		
		--Auditing Insert added 10/5/2014 - BAF
		Declare @PhoneRec VarChar(20) = Cast(@PatientPhoneID as VarChar(20))
		Declare @PatientPhone  VarChar(MAX) = ('PATIENT Phone Nbr: '+ @RecID + ', ' + @PhoneRec + @PatientPhoneNumberType + ', ' 
			+ @PatientAreaCode + ', ' + @PatientPhoneNumber)
		
		EXEC InsertAuditTrail @ChangeDate,'ALL',@PatientPhone,@PhoneRec,2,'BMTUpload','I'
		
		-- Insert patient info to the IndividualEMailAddress table
		INSERT INTO dbo.IndividualEMailAddress
		VALUES
		(
			@IndIdPatient,
			@PatientEmailType,
			@PatientEmail,
			1,
			'SYSTEM',
			GETDATE(),
			NULL
		);
		
		Select @IndEmailAddressID = SCOPE_IDENTITY();
		
		--Auditing Insert added 10/5/2014 - BAF
		Declare @EMailID VarChar(20) = Cast(@IndEmailAddressID as VarChar(20))
		Declare @PatientEmailAddr VarChar(Max) = ('PATIENT EMAIL: '+ @RecID + ', ' + @PatientEmailType + ', ' + 
			@PatientEmail)
		
		EXEC InsertAuditTrail @ChangeDate,'ALL',@PatientEmailAddr,@EMailID,2,'BMTUpload','I'
			
		-- Insert patient addres to the IndividualAddress table
		INSERT INTO dbo.IndividualAddress (IndividualID, Address1, Address2, Address3, City, State, Country, ZipCode,
			AddressType, IsActive, ModifiedBy, DateLastModified, UIC, LegacyID, DateVerified, VerifiedBy, ExpireDays)
		VALUES
		(
			@IndIdPatient, 
			@UnitAddress1,
			@UnitAddress2,
			@UnitAddress3,
			@cCity,
			@cState,
			@cCountry,
			@cZip,
			@cAddressType,
			1,
			'SYSTEM',
			GETDATE(),
			@UIC,
			NULL,
			NULL,
			NULL,
			NULL
		);
		-- Get the newly created ID for the patient address
		SELECT @IndAddressId = SCOPE_IDENTITY();
		
		--Auditing Insert added 10/5/2014 - BAF
		Declare @AddrID VarChar(20) = Cast(@IndAddressId as VarChar(20))
		Declare @PatAddress VarChar(Max) = ('PATIENT ADDRESS: '+ @RecID + ', ' + @UnitAddress1 + ', ' + @UnitAddress2 + ', ' +
			@UnitAddress3 + ', ' + @cCity + ', ' + @cState + ', ' +  @cCountry +
			', ' + @cZip + ', ' + @cAddressType)
		
		EXEC InsertAuditTrail @ChangeDate,'ALL',@PatAddress, @AddrID,2,'BMTUpload','I'
			
		-- Do Exam table insert
		INSERT INTO dbo.Exam (IndividualID_Patient, IndividualID_Examiner, ODCorrectedAcuity, ODUncorrectedAcuity,
			OSCorrectedAcuity, OSUncorrectedAcuity, ODOSCorrectedAcuity, ODOSUncorrectedAcuity, Comment,
			IsActive, ExamDate, ModifiedBy, DateLastModified, LegacyID)
		VALUES
		(
			@IndIdPatient,
			--@IndAddressId,
			@IndIdExaminer,
			@OdCorrectedAcuity,
			@OdUncorrectedAcuity,
			@OsCorrectedAcuity,
			@OsUncorrectedAcuity,
			@OdOsCorrectedAcuity,
			@OdOsUncorrectedAcuity,
			@ExamComment,
			1,
			@ExamDate,
			'SYSTEM',
			GETDATE(),
			Null
		);
		
		--Get the newly created Exam ID
		Select @IndExamID = SCOPE_IDENTITY();
		
		--Auditing Insert added 10/5/2014 - BAF
		Declare @ExamRec as VarChar(15) = Cast(@IndExamID as VarChar(15))
		Declare @ExamValue VarChar(Max) = ('EXAM: '+ @RecID + ', ' + @AddrID + ', ' 
			+ Cast(@IndIdExaminer as VarChar(20)) + ', ' + Cast(@OdCorrectedAcuity as VarChar(15)) + ', '
			+ Cast(@OdUncorrectedAcuity as VarChar(15)) + ', ' + Cast(@OsCorrectedAcuity as VarChar(15)) + ', ' + 
			Cast(@OsUncorrectedAcuity as VarChar(15)) + ', ' + Cast(@OdOsCorrectedAcuity as VarChar(15)) + ', ' + 
			Cast(@OdOsUncorrectedAcuity as VarChar(15)) + ', ' + @ExamComment + ', 1, ' + 
			Cast(@ExamDate as VarChar(20)) + ', SYSTEM, ' + Cast(@DateLastModified as VarChar(20)))
			
		EXEC InsertAuditTrail @ChangeDate, 'ALL',@ExamValue,@ExamRec,2,'BMTUpload','I'
		
		-- Do Prescription table inserts
		INSERT INTO dbo.Prescription
		(
			IndividualID_Doctor,
			IndividualID_Patient,
			ODSphere,
			OSSphere,
			ODCylinder,
			OSCylinder,
			ODAxis,
			OSAxis,
			ODHPrism,
			OSHPrism,
			ODVPrism,
			OSVPrism,
			ODHBase,
			OSHBase,
			ODVBase,
			OSVBase,
			ODAdd,
			OSAdd,
			EnteredODSphere,
			EnteredOSSphere,
			EnteredODCylinder,
			EnteredOSCylinder,
			EnteredODAxis,
			EnteredOSAxis,
			PrescriptionDate,
			IsUsed,
			IsActive,
			ModifiedBy,
			DateLastModified,
			PDDistant,
			PDNear,
			ODPDDistant, 
			ODPDNear, 
			OSPDDistant, 
			OSPDNear
		)
		VALUES
		(
			@IndIdProvider,
			@IndIdPatient,
			@OdSphere,
			@OsSphere,
			@OdCylinder,
			@OsCylinder,
			@OdAxis,
			@OsAxis,
			@OdHPrism,
			@OsHPrism,
			@OdVPrism,
			@OsVPrism,
			@OdHBase,
			@OsHBase,
			@OdVBase,
			@OsVBase,
			@OdAdd,
			@OsAdd,
			@OdSphere,
			@OsSphere,
			@OdCylinder,
			@OsCylinder,
			@OdAxis,
			@OsAxis,			
			@PrescriptionDate,
			1,
			1,
			'SYSTEM',
			GETDATE(),
			@PdDistant,
			@PdNear,
			@OdPdDistant,
			@OdPdNear,
			@OsPdDistant,
			@OsPdNear
		);
		-- Get the newly created ID for that prescription
		SELECT @PrescriptionId = SCOPE_IDENTITY();
		
		-- Auditing added 10/7/2014 -- BAF
		Declare @RXID as VarChar(20) = Cast(@PrescriptionId as VarChar(20))
		Declare @RXInfo VarChar(Max) = (@ExamRec + ', ' + @RecID + ', ' + Cast(@IndIdProvider as VarChar(15))
				 + ', ' + Cast(@ODSphere as VarChar(6)) + ', ' + Cast(@OSSphere as VarChar(6)) + ', ' + Cast(@ODCylinder as VarChar(6)) + ', ' + 
				 Cast(@OSCylinder as VarChar(6)) + ', ' + Cast(@ODAxis as VarChar(6)) + ', ' + Cast(@OSAxis as VarChar(6)) + ', ' + Cast(@ODHPrism as VarChar(6))
				 + ', ' + Cast(@OSHPrism as VarChar(6)) + ', ' + Cast(@ODVPrism as VarChar(6)) + ', ' + Cast(@OSVPrism as VarChar(6)) + ', ' + 
				 @ODHBase + ', ' + @OSHBase + ', ' + @ODVBase + ', ' + @OSVBase + ', ' + Cast(@ODAdd as VarChar(6)) + ', ' + Cast(@OSAdd as VarChar(6)) +
				 ', ' + Cast(@DateLastModified as VarChar(20)) + ', 1' +  ', ' + Cast(@ODSphere as VarChar(6)) + ', ' + 
				 Cast(@OSSphere as VarChar(6)) + ', ' + Cast(@ODCylinder as VarChar(6)) + ', ' + Cast(@OSCylinder as VarChar(6))
				  + ', ' + Cast(@ODAxis as VarChar(6)) + ', ' + Cast(@OSAxis as VarChar(6)) + ', SYSTEM,' + Cast(@DateLastModified as VarChar(20))
				   + ', 0, 0' + ', ' + Cast(@PdDistant as VarChar(8)) + ', ' + Cast(@PdNear as VarChar(8)) + ', ' + Cast(@OdPdDistant as VarChar(8)) 
				+ ', ' + Cast(@OdPdNear as VarChar(8)) + ', ' + Cast(@OsPdDistant as VarChar(8))
				+ ', ' + Cast(@OsPdNear as VarChar(8)))
			
		EXEC InsertAuditTrail @ChangeDate,'ALL',@RXInfo, @RXID,2,'BMTUpload','I'
		------------------------
		-- Do PatientOrder table inserts
		INSERT INTO dbo.PatientOrder
		(
			OrderNumber,
			ClinicSiteCode,
			IndividualID_Patient,
			IndividualID_Tech,
			PatientPhoneID,
			demographic,
			LensType,
			LensMaterial,
			Tint,
			Coatings, -- Added 1/3/18
			ODSegHeight,
			OSSegHeight,
			NumberOfCases,
			Pairs,
			PrescriptionID,
			FrameCode,
			FrameColor,
			FrameEyeSize,
			FrameBridgeSize,
			FrameTempleType,
			LabSiteCode,
			ShipToPatient,
			-- Ship Address is Clinic Address
			ShipAddress1,
			ShipAddress2,
			ShipAddress3,
			ShipCity, 
			ShipState, 
			ShipCountry,
			ShipZipCode, 
			
			ShipAddressType,
			LocationCode,
			UserComment1,
			UserComment2,
			IsGEyes, 
			IsMultivision,
			ODDistantDecenter, 
			OSDistantDecenter, 
			ODNearDecenter, 
			OSNearDecenter, 
			ODBase,
			OSBase,
			VerifiedBy,
			PatientEmail,
			OnholdForConfirmation, 
			IsActive,
			ModifiedBy,
			DateLastModified,
			LinkedID
		)
		VALUES
		(
			@OrderNumber,
			@ClinicSiteCode,
			@IndIdPatient,
			@IndIdTech,
			@PatientPhoneId,
			@Demographic,
			@LensType,
			@LensMaterial,
			@Tint,
			@LensCoating, -- Added 1/3/18
			@OdSegHeight,
			@OsSegHeight,
			@NumberOfCases,
			@NumberOfPairs,
			@PrescriptionId,
			@FrameCode,
			@FrameColor,
			@FrameEyeSize,
			@FrameBridgeSize,
			@FrameTempleType,
			@LabSiteCode,
			0,--@ShipToPatient,
			
			-- Ship Address is Clinic Address
			@cAddress1,
			@cAddress2,
			@cAddress3,
			@cCity,
			@cState,
			@cCountry,
			@cZip,
			@cAddressType,
			
			@LocationCode,
			@UserComment1,
			@UserComment2,
			0,
			@IsMultiVision,
			@OdDistantDeCenter,
			@OsDistantDeCenter,
			@OdNearDeCenter,
			@OsNearDeCenter,
			@OdBase,
			@OsBase,
			@VerifiedBy,
			@PatientEmail,
			0,
			1,
			'SYSTEM',
			@DateOrderCreated,
			@LinkedID
		);
		
		-- Auditing Added 10/7/14 - BAF
		DECLARE @OrderInfo VarChar(MAX) = ('Patient Order: ' + @OrderNumber + ', ' + @ClinicSiteCode + ', ' + @RecID 
			+ ', ' + Cast(@IndIdTech as VarChar(15)) + ', ' + @PhoneRec + ', ' + @Demographic + ', ' + @LensType + ', ' + @LensMaterial
			+ ', ' + @Tint + ', ' + @OdSegHeight + ', ' + @OsSegHeight + ', ' + Cast(@NumberOfCases as VarChar(3)) + ', ' + 
			Cast(@NumberOfPairs as VarChar(3)) + ', ' + @RXID + ', ' + @FrameCode + ', ' + @FrameColor + ', ' + @FrameEyeSize + ', ' + 
			@FrameBridgeSize + ', ' + @FrameTempleType + ', ' + @LabSiteCode + ', 0, ' + @cAddress1 + ', ' +
			@cAddress2 + ', ' + @cAddress3 + ', ' + @cCity + ', ' + @cState + ', ' + @cZip + ', ' + @cAddressType
			+ ', ' + @LocationCode + ', ' + @UserComment1 + ', ' + @UserComment2 + ', 0, ' + Cast(@IsMultiVision as VarChar(1))
			+ ', ' + Cast(@OdDistantDeCenter as VarChar(8)) + ', ' + 
			Cast(@OsDistantDeCenter as VarChar(8)) + ', ' + Cast(@OdNearDeCenter as VarChar(8))
			+ ', ' + Cast(@OsNearDeCenter as VarChar(8)) + ', ' + @OdBase + ', ' + @OsBase+ ', ' + 
			Cast(@VerifiedBy as VarChar(15)) + ', ' + @PatientEmail
			+ ', 0, 1, SYSTEM ' + Cast(@DateOrderCreated as VarChar(20)) + ', ' + @LinkedID + ', ' + @LensCoating)
		
		EXEC InsertAuditTrail @ChangeDate,'ALL',@OrderInfo, @OrderNumber ,2,'BMTUpload','I'
 		
		-- COMMIT ALL INSERTS (TRANSACTIONS) TO THE DB
		COMMIT TRANSACTION
		
		-- Do PatientOrder table updates to the DeCenter fields
		BEGIN
			Execute dbo.UpdatePatientOrder_Decenter @OrderNumber
													--@LensType,
													--@FrameEyeSize, 
													--@FrameBridgeSize, 
													--@ODPDDistant,
													--@OSPDDistant, 
													--@ODPDNear, 
													--@OSPDNear
													
		END
		
		BEGIN
			Declare @cODDistantDec VarChar(8) = Cast(@OdDistantDeCenter as VarChar(8)),
				@cOSDistantDec VarChar(8) = Cast(@OsDistantDeCenter as VarChar(8)),
				@cODNearDec VarChar(8) = Cast(@OdNearDeCenter as VarChar(8)),
				@cOSNearDec VarChar(8) = Cast(@OsNearDeCenter as VarChar(8))

			EXEC InsertAuditTrail @ChangeDate,'ODDistantDecenter',@cODDistantDec, @OrderNumber, 2,'InsertBulkOrder','U';
			EXEC InsertAuditTrail @ChangeDate,'OSPDDistantDecenter',@cOSDistantDec, @OrderNumber, 2,'InsertBulkOrder','U';
			EXEC InsertAuditTrail @ChangeDate,'ODPDNearDecenter',@cODNearDec, @OrderNumber, 2,'InsertBulkOrder','U';
			EXEC InsertAuditTrail @ChangeDate,'OSPDNearDecenter',@cOSNearDec, @OrderNumber, 2,'InsertBulkOrder','U';
		END
		
	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
		END

		EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		RETURN
	END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPatientInfoByIDNumber]    Script Date: 10/03/2019 13:59:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<David Blott>
-- Create date: <27 Nov 2013>
-- Description:	<Gets patient info by IDNumber>
-- Changes:  <1.>  <10/1/2014> <bf> <Changes were made to enable search by various ID Number Types>
--		<2.>  
-- =============================================
CREATE PROCEDURE [dbo].[GetAllPatientInfoByIDNumber]
@IDNumber VarChar(15),
@IDType VarChar(3)  -- Added 10/1/14 

--@IsActive bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	/* Auditing added to individual stored procedures listed below
		not required in this sp.  10/15/14 - BAF
	*/

	SET NOCOUNT ON;
	
-- Gets the individual ID number.
DECLARE @IndividualID INT = 0


	SELECT @IndividualID = IndividualID
	FROM dbo.IndividualIDNumber
	WHERE IDNumber = @IDNumber
		and IDNumberType = @IDType -- Added 10/1/14
--		and IsActive = @IsActive
	
	Declare @ModifiedBy VarChar(200) = Cast(@IndividualID as VarChar(200))
	
	IF(@IndividualID > 0)
	BEGIN
		EXEC dbo.GetIndividualByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualIDNumbersByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualEMailAddressesByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualPhoneNumbersByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetIndividualAddressesByIndividualID @IndividualID, 1, @ModifiedBy
		EXEC dbo.GetPatientOrderByIndividualID @IndividualID, @ModifiedBy
		EXEC dbo.GetExamsByIndividualID @IndividualID, @ModifiedBy
		EXEC dbo.GetPrescriptionsByIndividualID @IndividualID, @ModifiedBy
		EXEC dbo.GetIndividualTypesByIndividualID @IndividualID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DeletePatientOrder]    Script Date: 10/03/2019 13:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Barb Fieldhausen
-- Create date: 3/21/16
-- Description:	Delete a PatientOrder before lab receipt.
--		Insert a new status (14), set IsUsed flag for prescription
--		if this order is the only one using that prescription
-- =============================================
CREATE PROCEDURE [dbo].[DeletePatientOrder]
	-- Add the parameters for the stored procedure here
	@OrderNumber VarChar(16),
	@ModifiedBy VarChar(200),
	@Success BIT OUTPUT
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Get Prescription ID and count of orders with this RxID
	DECLARE @RxCnt INT,
		@RxID INT,
		@LabSiteCode VARCHAR(6),
		@ErrorLogID INT = 0,
		@LinkCnt INT = 0, 
		@LinkedID VarChar(16) = ''
	
	SELECT @RxId = PrescriptionID, @LabSiteCode = LabSiteCode FROM dbo.PatientOrder WHERE ORderNumber = @OrderNumber
	SELECT @RxCnt = COUNT(ORderNumber) FROM dbo.PatientOrder WHERE PrescriptionID = @RxID
	Select @LinkedID = LinkedID from dbo.PatientOrder where OrderNumber = @OrderNumber
	If @LinkedID <> '' 
		Set @LinkCnt = 1
	Else
		Begin
		SELECT @LinkCnt = COUNT(LinkedID) FROM dbo.PatientOrder where LinkedID = @OrderNumber
		End

	BEGIN TRY
	BEGIN TRANSACTION

		-- Update the order to inactive
		UPDATE dbo.PatientORder SET ISActive = 0, ModifiedBy = @ModifiedBy, DateLastModified = GETDATE() WHERE OrderNumber = @OrderNumber
		
		-- Insert new order status -- first set all current statuses to 0
		UPDATE dbo.PatientOrderStatus SET IsActive = 0 WHERE OrderNumber = @OrderNumber;
		
		Exec dbo.InsertPatientOrderStatus @OrderNumber,14,'Clinic Deleted',@LabSiteCode,1,@ModifiedBy,0,0
		--INSERT INTO dbo.PatientOrderStatus (OrderNumber, OrderSTatusTypeID, StatusComment, LabSiteCode, IsActive, ModifiedBy, 
		--	DateLastModified, LegacyID)
		--VALUES (@OrderNumber, 14,'Clinic Deleted Order', @LabSiteCode, 1, @ModifiedBy, GETDATE(),0)
		
		-- Only update the IsUsed Flag on the Prescription if there is only one order with this PrescriptionID
		IF @RxCnt = 1
		BEGIN
			UPDATE dbo.Prescription SET IsUsed = 0 WHERE ID = @RxID
		End

		--  Remove This order number from all LinkedIDs
		IF @LinkCnt >= 1
		BEGIN
			Select @LinkedID = LinkedID from dbo.PatientOrder where OrderNumber = @OrderNumber
			UPDATE dbo.PatientOrder SET LinkedID = '' where LinkedID = @OrderNumber
			Update dbo.PatientOrder set LinkedID = '' where LinkedID = @LinkedID
		END

		SET @Success = 1
		
	COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
	   IF XACT_STATE() <> 0
		BEGIN
			ROLLBACK TRANSACTION;
			EXECUTE dbo.InsertSQLErrorLog @ErrorLogID = @ErrorLogID OUTPUT;
		END
		-- Insert Failed
		SET @Success = 0
		RETURN
	END CATCH
END
GO
