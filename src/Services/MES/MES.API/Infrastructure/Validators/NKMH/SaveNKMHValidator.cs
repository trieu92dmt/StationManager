using FluentValidation;
using MES.Application.Commands.MES;

namespace MES.API.Infrastructure.Validators.NKMH
{
    public class SaveNKMHValidator : AbstractValidator<SaveNKMHCommand>
    {
        public SaveNKMHValidator()
        {
            RuleForEach(x => x.NKMHRequests).SetValidator(new SaveNKMHItemValidator());
        }
    }

    public class SaveNKMHItemValidator : AbstractValidator<NKMHRequest>
    {
        public SaveNKMHItemValidator()
        {
            RuleFor(x => x.BagQuantity).GreaterThan(0).WithMessage("Số lượng bao phải lớn hơn 0");
            RuleFor(x => x.SingleWeight).GreaterThan(0).WithMessage("Đơn trọng phải lớn hơn 0");
        }
    }
}
 