using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;

namespace TransactionService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound(new { Message = $"Transaction with ID {id} not found." });

            return Ok(transaction);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> Filter([FromQuery] TransactionFilterDto filter)
        {
            var transactions = await _transactionService.FilterTransactionsAsync(filter);
            return Ok(transactions);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByProductId(int productId)
        {
            var transactions = await _transactionService.GetTransactionsByProductIdAsync(productId);
            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var createdTransaction = await _transactionService.CreateTransactionAsync(transactionDto);
                return CreatedAtAction(nameof(GetById), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionDto transactionDto)
        {
            if (id != transactionDto.Id)
                return BadRequest(new { Message = "ID in URL does not match ID in request body." });

            try
            {
                var result = await _transactionService.UpdateTransactionAsync(transactionDto);
                if (!result)
                    return NotFound(new { Message = $"Transaction with ID {id} not found." });

                return Ok(new { Message = "Transaction updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _transactionService.DeleteTransactionAsync(id);
                if (!result)
                    return NotFound(new { Message = $"Transaction with ID {id} not found." });

                return Ok(new { Message = "Transaction deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}