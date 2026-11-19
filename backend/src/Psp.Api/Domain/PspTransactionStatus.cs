namespace Psp.Api.Domain;

public enum PspTransactionStatus
{
    Created,
    Pending,
    Succeeded,
    Failed,
    Error,
    Expired,
}