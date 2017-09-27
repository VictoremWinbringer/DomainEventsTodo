using DomainEventsTodo.Domain;
using DomainEventsTodo.Filters;
using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace DomainEventsTodo.Controllers
{
    [Route("api/v1/[controller]")]
    [ValidateModel]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;

        public TodoController(ITodoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_repository.All().Select(TodoVm.FromTodo).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(TodoVm.FromTodo(_repository[id]));
        }

        [HttpPost]
        public IActionResult Post([FromBody]TodoPostPutVm todo)
        {
            var result = new Todo(todo.Description);

            _repository.Add(result);

            return CreatedAtAction(nameof(TodoController.Get), new { id = result.Id }, TodoVm.FromTodo(result));
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]TodoPostPutVm todo)
        {
            var result = _repository[id];

            result.Update(todo.Description);

            _repository.Replace(result);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _repository.Remove(id);

            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult Count()
        {
            return Ok(_repository.All().Count());
        }

        [HttpPost("{id}/[action]")]
        public IActionResult MakeComplete(Guid id)
        {
            var result = _repository[id];

            result.MakeComplete();

            _repository.Replace(result);

            return Ok();
        }
    }
}
