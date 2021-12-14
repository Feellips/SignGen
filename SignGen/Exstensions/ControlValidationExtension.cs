using SignGen.Validators;

namespace SignGen.Exstensions
{
    internal static class ControlValidationExtension
    {
        public static ArgsValidator Rules(this string[] args) => new ArgsValidator(args);
    }
}
