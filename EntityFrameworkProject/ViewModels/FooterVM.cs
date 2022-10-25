using EntityFrameworkProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.ViewModels
{
    public class FooterVM
    {
        public string Email { get; set; }
        public IEnumerable<Social> Socials { get; set; }
    }
}
