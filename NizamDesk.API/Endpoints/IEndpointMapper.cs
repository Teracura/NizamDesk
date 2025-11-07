namespace NizamDesk.API.EndPoints;

public interface IEndpointMapper
{
    public static abstract Task Map(IEndpointRouteBuilder app);
}