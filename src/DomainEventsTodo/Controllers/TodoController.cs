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
            return Ok(_repository.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var todo = _repository[id];

            if (todo == null)
                return NotFound();

            return Ok(TodoVm.FromTodo(todo));
        }

        [HttpPost]
        public IActionResult Post([FromBody]TodoPostPutVm todo)
        {
            var result = new Todo(todo.Description);

            _repository.Add(result);

            return Ok(TodoVm.FromTodo(result));
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]TodoPostPutVm todo)
        {
            var result = _repository[id];

            if (result == null)
                return NotFound();

            result.Update(todo.Description);

            _repository.Replace(result);

            return Ok();
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
            return Ok(_repository.Count());
        }

        [HttpPost("{id}/[action]")]
        public IActionResult MakeComplete(Guid id)
        {
            var result = _repository[id];

            if (result == null)
                return NotFound();

            result.MakeComplete();

            _repository.Replace(result);

            return Ok();
        }
    }
}
