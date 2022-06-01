using System;
using System.Collections.Generic;

namespace SrtsWeb.Entities
{
    [Serializable]
    public sealed class PatientEntity
    {
        public IndividualEntity Individual { get; set; }

        public List<IdentificationNumbersEntity> IDNumbers { get; set; }

        public List<AddressEntity> Addresses { get; set; }

        public List<PhoneNumberEntity> PhoneNumbers { get; set; }

        public List<EMailAddressEntity> EMailAddresses { get; set; }

        public List<ExamEntity> Exams { get; set; }

        public List<OrderEntity> Orders { get; set; }

        public List<PrescriptionEntity> Prescriptions { get; set; }

        public List<IndividualTypeEntity> IndividualTypes { get; set; }
    }
}