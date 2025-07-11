using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MiniCMMS.Dtos;

public class RegisterDto : IValidatableObject
{
    [Required]
    [RegularExpression(@"^[a-zA-Z]+([ -][a-zA-Z]+)*$", ErrorMessage = "First name can only contain letters, spaces, or dashes (not at the end).")]
    [MaxLength(50)]
    public string FirstName { get; set; } = "";

    [Required]
    [RegularExpression(@"^[a-zA-Z]+([ -][a-zA-Z]+)*$", ErrorMessage = "Last name can only contain letters, spaces, or dashes (not at the end).")]
    [MaxLength(50)]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = "";


    [Required]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters long.")]
    public string Password { get; set; } = "";

    [Required]
    [RegularExpression(@"^(Manager|Technician)$", ErrorMessage = "Role must be either Manager or Technician.")]
    public string Role { get; set; } = "Technician";


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var inputFields = new Dictionary<string, string>
        {
            { nameof(FirstName), FirstName },
            { nameof(LastName), LastName },
            { nameof(Email), Email },
            { nameof(Password), Password }
        };

        foreach (var field in inputFields)
        {
            if (ContainsEmoji(field.Value))
            {
                yield return new ValidationResult("Input contains unsupported characters (e.g. emojis)", new[] { field.Key });
            }
        }
    }

    private bool ContainsEmoji(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return input.Any(c => char.IsSurrogate(c) || char.GetUnicodeCategory(c) == UnicodeCategory.OtherSymbol);
    }

    public static string CapitalisedName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        var separators = new[] { ' ', '-' };
        var parts = name.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        var capitalisedParts = parts
            .Select(p => char.ToUpper(p[0]) + p[1..].ToLower())
            .ToArray();

        int index = 0;
        var result = "";

        for (int i = 0; i < name.Length; i++)
        {
            if (separators.Contains(name[i]))
            {
                result += name[i];
            }
            else
            {
                if (index < capitalisedParts.Length)
                {
                    result += capitalisedParts[index];
                    i += parts[index].Length - 1;
                    index++;
                }
            }
        }
        return result;
    }


}