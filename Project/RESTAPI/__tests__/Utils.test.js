const Utils = require('../crudOps/Utils');

describe('Utils', () => {
  describe('isDefault', () => {
    test('returns true for null', () => {
      expect(Utils.isDefault(null)).toBe(true);
    });

    test('returns true for undefined', () => {
      expect(Utils.isDefault(undefined)).toBe(true);
    });

    test('returns true for empty string', () => {
      expect(Utils.isDefault("")).toBe(true);
    });

    test('returns true for 0', () => {
      expect(Utils.isDefault(0)).toBe(true);
    });

    test('returns false for non-empty string', () => {
      expect(Utils.isDefault("hello")).toBe(false);
    });

    test('returns false for non-zero number', () => {
      expect(Utils.isDefault(42)).toBe(false);
    });

    test('returns false for object', () => {
      expect(Utils.isDefault({})).toBe(false);
    });
  });

  describe('Convert', () => {
    test('converts varchar value to string', () => {
      const result = Utils.Convert(123, { declaration: 'varchar' });
      expect(result).toBe("123");
    });

    test('converts int value to number', () => {
      const result = Utils.Convert("42", { declaration: 'int' });
      expect(result).toBe(42);
    });

    test('converts float value to number', () => {
      const result = Utils.Convert("3.14", { declaration: 'float' });
      expect(result).toBe(3.14);
    });

    test('converts date value', () => {
      const result = Utils.Convert("2024-01-15", { declaration: 'date' });
      expect(result instanceof Date).toBe(true);
    });

    test('converts datetime value', () => {
      const result = Utils.Convert("2024-01-15T10:30:00", { declaration: 'datetime' });
      expect(result instanceof Date).toBe(true);
    });

    test('converts boolean true', () => {
      const result = Utils.Convert(true, { declaration: 'boolean' });
      expect(result).toBe(true);
    });

    test('converts boolean false', () => {
      const result = Utils.Convert(false, { declaration: 'boolean' });
      expect(result).toBe(false);
    });
  });

  describe('constants', () => {
    test('SELECT is 0', () => {
      expect(Utils.SELECT).toBe(0);
    });

    test('UPDATE is 1', () => {
      expect(Utils.UPDATE).toBe(1);
    });

    test('INSERT is 2', () => {
      expect(Utils.INSERT).toBe(2);
    });

    test('DELETE is 3', () => {
      expect(Utils.DELETE).toBe(3);
    });
  });
});
