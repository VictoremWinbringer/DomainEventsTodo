using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;
using DomainEventsTodo.Dispatchers;
using DomainEventsTodo.Domain;
using DomainEventsTodo.Domain.Events;
using DomainEventsTodo.Repositories.Abstract;

namespace DomainEventsTodo.Repositories
{
    internal sealed class TodoRepository : ITodoRepository
    {
        private readonly ISession _session;
        private readonly IDispatcher _dispatcher;

        public TodoRepository(ISession session, IDispatcher dispatcher)
        {
            _session = session;
            _dispatcher = dispatcher;
        }

        public IEnumerable<Todo> All()
        {
            RowSet rows = _session.Execute("select id, description, iscomplete from todos;");

            foreach (Row row in rows)
                yield return Create(row);
        }

        public Todo this[Guid id] => Get(id);

        public void Remove(Guid id)
        {
            Execute("delete from todos where id =?;", id);
        }

        public void Replace(Todo todo)
        {
            Execute(
                "update todos set iscomplete =?, description =? where id =?;" /* IF EXISTS;" */
                , todo.IsComplete, todo.Description, todo.Id);

            Dispatch(todo.Events);
        }

        public void Add(Todo todo)
        {
            Execute(
                "insert into todos (id, description, iscomplete) values (?,?,?);"
                , todo.Id, todo.Description, todo.IsComplete);

            Dispatch(todo.Events);
        }

        #region private methods

        private void Dispatch(IEnumerable<BaseEvent> events)
        {
            foreach (var e in events)
            {
                _dispatcher.Dispatch(e);
            }
        }

        private Todo Create(Row row)
        {
            return new Todo(
                (Guid) row["id"],
                (string) row["description"],
                (bool) row["iscomplete"]
                );
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

        #endregion
    }
}