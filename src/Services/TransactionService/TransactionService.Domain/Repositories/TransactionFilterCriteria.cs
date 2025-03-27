using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Domain.Repositories
{
    public class TransactionFilterCriteria
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TransactionType { get; set; }
        public int? ProductId { get; set; }
    }
}
