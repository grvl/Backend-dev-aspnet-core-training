import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../environments/environment';
import { List } from '../_models/list';
import { PaginatedObject } from '../_models/paginatedObject';
import { ObjectPagination } from '../_models/ObjectPagination';
import { Observable } from 'rxjs';
import { ListWithPaginatedItems } from '../_models/listWithPaginatedItems';

@Injectable({ providedIn: 'root' })
export class ListService {
    constructor(private http: HttpClient) { }

    getAll(url?: string, objectPagination?: ObjectPagination):Observable<PaginatedObject<List>> {
      if (url){
        return this.http.get<PaginatedObject<List>>(`${environment.apiUrl}/${url}`);
      }

      if (objectPagination){
        return this.http.get<PaginatedObject<List>>(`${environment.apiUrl}/List?size=${objectPagination.size}&page=${objectPagination.page}`);
      }

      return this.http.get<PaginatedObject<List>>(`${environment.apiUrl}/List`);

    }

    getById(id: number, url?: string, objectPagination?: ObjectPagination): Observable<ListWithPaginatedItems> {
      if (url){
        return this.http.get<ListWithPaginatedItems>(`${environment.apiUrl}/${url}`);
      }

      if (objectPagination){
        return this.http.get<ListWithPaginatedItems>(`${environment.apiUrl}/List/${id}?size=${objectPagination.size}&page=${objectPagination.page}`);
      }
        return this.http.get<ListWithPaginatedItems>(`${environment.apiUrl}/List/${id}`);
    }

    create(list: List) {
        return this.http.post(`${environment.apiUrl}/List`, list);
    }

    update(list: List): Observable<List> {
      console.log(list);
        return this.http.put<List>(`${environment.apiUrl}/List/${list.listId}`, list);
    }

    delete(id: number) {
        return this.http.delete(`${environment.apiUrl}/List/${id}`);
    }

    share(id: number, userId:number) {
      return this.http.get(`${environment.apiUrl}/List/${id}/${userId}`);
    }
}
