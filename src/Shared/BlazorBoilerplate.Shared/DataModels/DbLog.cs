﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorBoilerplate.Shared.DataModels
{
    public class DbLog
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        public string Level { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Exception { get; set; }

        public string Properties { get; set; }

        [NotMapped]
        public IDictionary<string, string> LogProperties { get;set;}

    }
}