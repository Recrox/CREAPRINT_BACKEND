using System;

namespace CreaPrintCore.Models
{
    [Flags]
    public enum UserRights
    {
        None = 0,
        Admin = 1,
        Vendeur = 2,
        Artisan = 4,
        Commercial = 8,
        // add other rights here as needed
    }
}
