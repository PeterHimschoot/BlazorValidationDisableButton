using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorValidation.Shared
{
  public class Customer
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Street is required")]
    [StringLength(50)]
    public string Street { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(50)]
    public string City { get; set; }

  }
}
