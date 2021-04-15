using System.Collections.Generic;

namespace DeviceDbModel
{
  public static class Roles
  {
    public const string SuperAdmin = "super_admin";
    public const string Admin = "admin";
    public const string Dealer = "dealer ";
    public const string DealerAdmin = "dealer_admin";
    public const string Owner = "owner";
    public const string Technician = "technician";

    public static List<RolePrm> RolesPermissions = new()
    {
      new RolePrm {Name = SuperAdmin, Code = 0},
      new RolePrm {Name = Admin, Code = 1},
      new RolePrm {Name = Dealer, Code = 2},
      new RolePrm {Name = DealerAdmin, Code = 3},
      new RolePrm {Name = Owner, Code = 4},
      new RolePrm {Name = Technician, Code = 5}
    };

    //public const string User = "ROLE_USER";
  }

  public class RolePrm
  {
    public string Name { get; set; }

    public int Code { get; set; }
  }

  //public static class RolesPermissions
  //{
  //  public static (string, int) SuperAdmin => ("super_admin", 0);

  //  public static (string, int) Admin => ("admin", 1);

  //  public static (string, int) Dealer => ("dealer", 2);

  //  public static (string, int) DealerAdmin => ("dealer_admin", 3);

  //  public static (string, int) Owner => ("owner", 4);

  //  public static (string, int) Technician => ("technician", 5);
  //}
}
