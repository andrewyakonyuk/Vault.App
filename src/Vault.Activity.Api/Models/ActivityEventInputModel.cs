using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vault.Shared;

namespace Vault.Activity.Api.Models
{
    public class ActivityEventInputModel
    {
        public ActivityEventInputModel()
        {
            MetaBag = new JObject();
        }
        
        public dynamic MetaBag { get; set; }
        
        [Required]
        public string Id { get; set; }
        
        public string Verb { get; set; }
        
        public string Actor { get; set; }
        
        public string Target { get; set; }
        
        public DateTimeOffset Published { get; set; }
        
        [Required]
        public string Provider { get; set; }
        
        public string Content { get; set; }
        
        public string Title { get; set; }
        
        public string Uri { get; set; }
    }
}
