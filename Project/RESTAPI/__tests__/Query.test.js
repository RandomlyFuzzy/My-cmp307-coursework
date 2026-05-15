const Utils = require('../crudOps/Utils');

jest.mock('../crudOps/interface', () => {
  const mockGetResponse = jest.fn().mockResolvedValue([{ id: 1 }]);
  const mockGetResponseNoneQuery = jest.fn().mockResolvedValue([1]);
  const mockInterface = jest.fn().mockResolvedValue({
    GetResponse: mockGetResponse,
    GetResponseNoneQuery: mockGetResponseNoneQuery,
  });
  return mockInterface;
});

describe('Query', () => {
  let Query;

  beforeAll(() => {
    Query = require('../crudOps/Query');
  });

  test('constructs a SELECT query', async () => {
    const q = new Query(null, Utils.SELECT, 'Twang.Users', {
      id: { value: 1, type: { declaration: 'int' } },
    });
    const result = await q.GetResult();
    expect(result).toEqual([{ id: 1 }]);
  });

  test('constructs a SELECT query with no values (all rows)', async () => {
    const q = new Query(null, Utils.SELECT, 'Twang.Users', {});
    const result = await q.GetResult();
    expect(result).toEqual([{ id: 1 }]);
  });

  test('constructs an INSERT query', async () => {
    const q = new Query(null, Utils.INSERT, 'Twang.Room', {
      id: { value: null, type: { declaration: 'int' } },
      rname: { value: 'Test Room', type: { declaration: 'varchar' } },
      capacity: { value: 10, type: { declaration: 'int' } },
    });
    const result = await q.GetResult();
    expect(result).toEqual([1]);
  });

  test('constructs an UPDATE query', async () => {
    const q = new Query(
      null,
      Utils.UPDATE,
      'Twang.Room',
      {
        id: { value: 1, type: { declaration: 'int' } },
        rname: { value: 'Updated Room', type: { declaration: 'varchar' } },
      },
      'id',
      1
    );
    const result = await q.GetResult();
    expect(result).toEqual([1]);
  });

  test('constructs a DELETE query', async () => {
    const q = new Query(
      null,
      Utils.DELETE,
      'Twang.Room',
      { id: { value: 1, type: { declaration: 'int' } } },
      'id',
      1
    );
    const result = await q.GetResult();
    expect(result).toEqual([1]);
  });
});
