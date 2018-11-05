using Microsoft.ML.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models.ML
{
    public class PassengerData
    {
        [Column("0")]
        public float Gender;

        [Column("1")]
        public float Nationality;

        [Column("2")]
        public float year;

        [Column("3")]
        [ColumnName("Label")]
        public string Label;
    }
}
