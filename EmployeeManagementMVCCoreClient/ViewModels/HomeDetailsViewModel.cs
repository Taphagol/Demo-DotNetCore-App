using EmployeeManagementMVCCoreClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementMVCCoreClient.ViewModels
{
    public class HomeDetailsViewModel
    {
        public Employee employee { get; set; }
        public string pageTitle { get; set; }
    }
}
