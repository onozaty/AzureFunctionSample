using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AzureFunctionSample.Data;
using AzureFunctionSample.Models;
using System.Text.Json;

namespace AzureFunctionSample;

public class TodoFunctions
{
    private readonly ILogger<TodoFunctions> _logger;
    private readonly AppDbContext _dbContext;

    public TodoFunctions(ILogger<TodoFunctions> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [Function("GetTodos")]
    public async Task<IActionResult> GetTodos(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest req)
    {
        _logger.LogInformation("Getting all todos");
        
        var todos = await _dbContext.Todos.ToListAsync();
        return new OkObjectResult(todos);
    }

    [Function("GetTodo")]
    public async Task<IActionResult> GetTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos/{id}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation("Getting todo with id: {Id}", id);
        
        var todo = await _dbContext.Todos.FindAsync(id);
        
        if (todo == null)
        {
            return new NotFoundResult();
        }
        
        return new OkObjectResult(todo);
    }

    [Function("CreateTodo")]
    public async Task<IActionResult> CreateTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] HttpRequest req)
    {
        _logger.LogInformation("Creating a new todo");
        
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var todo = JsonSerializer.Deserialize<Todo>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (todo == null || string.IsNullOrWhiteSpace(todo.Title))
        {
            return new BadRequestObjectResult("Invalid todo data");
        }
        
        todo.CreatedAt = DateTime.UtcNow;
        
        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync();
        
        return new CreatedResult($"/api/todos/{todo.Id}", todo);
    }

    [Function("UpdateTodo")]
    public async Task<IActionResult> UpdateTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todos/{id}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation("Updating todo with id: {Id}", id);
        
        var existingTodo = await _dbContext.Todos.FindAsync(id);
        
        if (existingTodo == null)
        {
            return new NotFoundResult();
        }
        
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var updatedTodo = JsonSerializer.Deserialize<Todo>(requestBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (updatedTodo == null || string.IsNullOrWhiteSpace(updatedTodo.Title))
        {
            return new BadRequestObjectResult("Invalid todo data");
        }
        
        existingTodo.Title = updatedTodo.Title;
        existingTodo.IsCompleted = updatedTodo.IsCompleted;
        
        await _dbContext.SaveChangesAsync();
        
        return new OkObjectResult(existingTodo);
    }

    [Function("DeleteTodo")]
    public async Task<IActionResult> DeleteTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todos/{id}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation("Deleting todo with id: {Id}", id);
        
        var todo = await _dbContext.Todos.FindAsync(id);
        
        if (todo == null)
        {
            return new NotFoundResult();
        }
        
        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();
        
        return new NoContentResult();
    }
}
