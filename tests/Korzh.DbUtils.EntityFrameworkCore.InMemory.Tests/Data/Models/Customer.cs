﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Korzh.DbUtils.EntityFrameworkCore.InMemory.Tests.Models
{

    [DisplayColumn("Name")]
    public class Customer
    {
        [Column("CustomerID")]
        public string Id { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}