namespace DotnetSampleApp.Application.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"Entity '{entityName}' with key '{key}' was not found.");
