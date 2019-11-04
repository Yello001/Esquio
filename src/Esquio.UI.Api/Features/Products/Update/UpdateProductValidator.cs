﻿using FluentValidation;

namespace Esquio.UI.Api.Features.Products.Update
{
    public class UpdateProductValidator
        : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.CurrentName)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(200)
                .Matches(ApiConstants.Constraints.NamesRegularExpression);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(200)
                .Matches(ApiConstants.Constraints.NamesRegularExpression);

            RuleFor(x => x.Description)
                .MaximumLength(2000);
        }
    }
}
