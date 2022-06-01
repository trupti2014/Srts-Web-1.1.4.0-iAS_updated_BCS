using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace SrtsWeb.Account
{
    public class CertificateInfo
    {
        private String _friendlyName;
        private String _cn;

        private String _firstName;
        private String _lastName;
        private String _middleName;
        private String _number;
        private String[] _nameInfo;

        private String[] _info;
        private String _ouA;
        private String _ouB;
        private String _ouC;
        private String _o;
        private String _c;


        public CertificateInfo1(X509Certificate2 certificate)
        {
            setProperties(certificate);

        }

        private void setProperties(X509Certificate2 cert)
        {
            char[] delim = { ',' };
            char[] delim2 = { '.' };

            _info = cert.Subject.Split(delim);
            _cn = _info[0].Substring(3);
            _nameInfo = _cn.Split(delim2);
            setNameProperties(_nameInfo);
            _ouA = _info[1];
            _ouB = _info[2];
            _ouC = _info[3];
            _o = _info[4];
            _c = _info[5];

            _friendlyName = cert.FriendlyName;
        }

        private void setNameProperties(string[] nameProps)
        {
            _number = nameProps[nameProps.Length - 1];
            _lastName = nameProps[0];
            _firstName = nameProps[1];
            _middleName = checkMiddleName(nameProps[2]);
        }

        private String checkMiddleName(string mName)
        {
            String returnName = String.Empty;
            int tpOut;

            if (!int.TryParse(mName, out tpOut))
            {
                returnName = mName;
            }

            return returnName;
        }

        
    }
}
