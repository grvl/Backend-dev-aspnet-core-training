import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Item } from '../_models/item';

import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ItemService {
    constructor(private http: HttpClient) { }

    getItem(id: number): Observable<Item>{
      return this.http.get<Item>(`${environment.apiUrl}/item/${id}`);
    }

    editItem(item: Item): Observable<Item>{
      return this.http.put<Item>(`${environment.apiUrl}/item/${item.itemId}`, item);
    }

    createItem(item: Item){
        return this.http.post<Item>(`${environment.apiUrl}/item`, item);
    }

    deleteItem(id: number){
      return this.http.delete<Item>(`${environment.apiUrl}/item/${id}`);
    }
}
