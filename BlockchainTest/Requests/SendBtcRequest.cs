using System;
using System.ComponentModel.DataAnnotations;

namespace BlockchainTest.Requests
{
    public class SendBtcRequest
    {
        [Required]
        public string Address { get; set; }
        [Range(0.00001, Double.PositiveInfinity, ErrorMessage = "Amount should be greater than 0.00001")]
        public decimal Amount { get; set; }
    }
}