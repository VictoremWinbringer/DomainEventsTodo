using Cassandra;
using DomainEventsTodo.Domain;
using DomainEventsTodo.Repositories.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DomainEventsTodo.Repositories.Concrete
{
    internal sealed class TodoRepository : ITodoRepository
    {
        private readonly ISession _session;

        public TodoRepository(ISession session)
        {
            _session = session;
        }
        public IEnumerator<Todo> GetEnumerator()
        {
            RowSet rows = _session.Execute("select id, description, iscomplete from todos;");

            foreach (Row row in rows)
                yield return Create(row);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Todo this[Guid id] => Get(id);

        private Todo Create(Row row)
        {
            return new Todo(new TodoMemento
            {
                Id = (Guid)row["id"],
                Description = (string)row["description"],
                IsComplete = (bool)row["iscomplete"]
            });
        }

        private Todo Get(Guid id)
        {
            Row row = _session.Execute(_session.Prepare("select * from todos where id=?")
                .Bind(id)).FirstOrDefault();

            if (row == null)
                return null;

            return Create(row);
        }

        private RowSet Execute(string sql, params object[] values)
        {
            var prepare = _session.Prepare(cqlQuery: sql);

            var statement = prepare.Bind(values: values);

            return _session.Execute(statement: statement);
        }

        public void Remove(Guid id)
        {
            Execute("delete from todos where id =?;", id);
        }

        public void Replace(Todo todo)
        {
            var memento = todo.Memento;

            Execute(
                "update todos set iscomplete =?, description =? where id =?;"/* IF EXISTS;" */
                , memento.IsComplete, memento.Description, memento.Id);
        }

        public void Add(Todo todo)
        {
            var memento = todo.Memento;

            Execute(
                "insert into todos (id, description, iscomplete) values (?,?,?);"
                , memento.Id, memento.Description, memento.IsComplete);
        }
    }
}
