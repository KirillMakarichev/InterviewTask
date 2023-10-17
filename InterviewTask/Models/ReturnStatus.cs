namespace InterviewTask.Models;

public class ReturnStatus<TResponse, TError>
{
    public TResponse Response { get; set; }
    public TError Error { get; set; }

    public static ReturnStatus<TResponse, TError> Create(TResponse response, TError error) =>
        new()
        {
            Response = response,
            Error = error
        };
}