using System.Collections.Generic;

namespace DeviceDbModel.Models
{
  public class Country
  {
    public Country() { Users = new HashSet<ApplicationUser>(); }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Alpha2Code { get; set; }

    public string Alpha3Code { get; set; }

    public int Code { get; set; }

    public ICollection<ApplicationUser> Users { get; set; }
  }
}
