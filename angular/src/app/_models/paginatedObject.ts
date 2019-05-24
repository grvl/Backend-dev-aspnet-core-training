
export class PaginatedObject<T> {
  total: number;
  totalPages: number;
  pageSize: number;
  pageNumber: number;
  result: T[];
  previous: string;
  next: string;
}
