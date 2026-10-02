using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
    }
    public ApplicationUser(string id, string name, string surname, string userName, string dni, int age, string sex)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
        Dni = dni;
        Age = age;
        Sex = sex;
    }

    [StringLength(50)]
    public string? Name { get; set; }
 
    [StringLength(50)]
    public string? Surname { get; set; }
 
    [StringLength(9, MinimumLength = 9, ErrorMessage = "El DNI debe tener 9 caracteres.")]
    public string? Dni { get; set; }
 
    [Range(0, 120, ErrorMessage = "La edad debe estar entre 0 y 120 años.")]
    public int Age { get; set; }
 
    [StringLength(1, ErrorMessage = "El sexo debe indicarse con un solo carácter.")]
    public string? Sex { get; set; }
}
