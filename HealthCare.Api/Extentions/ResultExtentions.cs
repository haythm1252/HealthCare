
using HealthCare.Application.Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Extentions;

public static class ResultExtentions
{
    extension(Result result)
    {
        public ObjectResult ToProblem()
        {
            var problem = Results.Problem(statusCode: result.Error.StatusCode);
            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
                    {
                        {
                            "errors", new[]
                            {
                                result.Error.Code ,
                                result.Error.Description
                            }
                        }
                    };

            return new ObjectResult(problemDetails);
        }
    }

}
