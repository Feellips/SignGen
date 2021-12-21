using SignGen.Demo.Validators;

namespace SignGen.Demo.Exstensions
{
    internal static class ControlValidationExtension
    {
        public static ArgsValidator Rules(this string[] args) => new ArgsValidator(args);
    }
}
