using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared.Commands;

namespace Vault.WebHost.Models.Boards
{
    public class CreateBoard : ICommandContext
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Query { get; set; }
    }
}