using SrtsWeb.BusinessLayer.Abstract;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.Entities;
using System;
using System.Collections.Generic;

namespace SrtsWeb.BusinessLayer.Concrete
{
    /// <summary>
    ///  Custom class used to perform Lookup operations against the lookup table.
    /// </summary>
    public sealed class LookupService : ILookupService
    {
        /// <summary>
        /// Gets a list of all lookup types.
        /// </summary>
        /// <returns>List of all lookup types.</returns>
        public List<LookupTableEntity> GetAllLookups()
        {
            var _repository = new LookupRepository();
            return _repository.GetAllLooksups();
        }

        /// <summary>
        /// Gets a list of lookup table items by a lookup type.
        /// </summary>
        /// <param name="type">The lookup type to find.</param>
        /// <returns>List of items that match the lookup type.</returns>
        public List<LookupTableEntity> GetLookupsByType(string type)
        {
            var _repository = new LookupRepository();
            return _repository.GetLookupsByType(type);
        }

        /// <summary>
        /// Add new items to the lookup table.
        /// </summary>
        /// <param name="LookUpTableItem">Item to add</param>
        /// <returns>Success/Failure of insert.</returns>
        public Boolean InsertLookUpTableItem(LookupTableEntity LookUpTableItem)
        {
            var _repository = new LookupRepository();
            return _repository.InsertLookUpTableItem(LookUpTableItem);
        }

        /// <summary>
        /// Update existing lookup table values.
        /// </summary>
        /// <param name="LookUpTableItem">Item to update.</param>
        /// <returns>List of updated items, it is really just one item.</returns>
        public List<LookupTableEntity> UpdateLookUpTable(LookupTableEntity LookUpTableItem)
        {
            var _repository = new LookupRepository();
            return _repository.UpdateLookUpTable(LookUpTableItem);
        }
    }
}