using DomainEventsTodo.Domain;
using DomainEventsTodo.Filters;
using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using DomainEventsTodo.Domain.Mementos;

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
            return Ok(_repository.All().Select(TodoDisplayVm.FromTodo).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute]TodoSearchVm todo)
        {
            return Ok(TodoDisplayVm.FromTodo(_repository[todo.Id]));
        }

        [HttpPost]
        public IActionResult Post([FromBody]TodoCreateVm todo)
        {
            var result = new Todo(todo.Description);

            _repository.Add(result);

            return CreatedAtAction(nameof(TodoController.Get), new { id = result.Id }, TodoDisplayVm.FromTodo(result));
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromRoute]TodoSearchVm search, [FromBody]TodoUpdateVm todo)
        {
            var result = _repository[search.Id];

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
        public IActionResult MakeComplete([FromRoute]TodoSearchVm todo)
        {
            var result = _repository[todo.Id];

            result.MakeComplete();

            _repository.Replace(result);

            return Ok();
        }
    }
}
