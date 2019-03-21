using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.Contracts;
using BlockchainTest.Requests;
using BlockchainTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainTest.Controllers
{
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionsController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("api/getlast")]
        public async Task<IActionResult> GetLast()
        {
            var lastTransactions = await _transactionService.GetLast();
            return new JsonResult(lastTransactions.Select(t => new Transaction()
            {
                Amount = t.Amount,
                Address = t.Address,
                Confirmations = t.Confirmations,
                Date = t.TimeReceived
            }));
        }

        [HttpPost("api/sendbtc")]
        public async Task<IActionResult> SendBtc([FromBody] SendBtcRequest req)
        {
            return new JsonResult(new SuccessResponse()
            {
                Success = await _transactionService.SendBtc(req.Address, req.Amount)
            });
        }
    }
}