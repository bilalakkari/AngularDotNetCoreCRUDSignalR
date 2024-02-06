import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private readonly url = 'https://localhost:7060/api/User'

  constructor(
    private api: HttpClient,
  ) { }

  getAllUsers() {
    return this.api.get(`${this.url}/GetAllUsers`)
  }

  addNewUser(data: any) {
    return this.api.post(`${this.url}/AddNewUser`, data)
  }


  deleteUser(data: any, headers: any) {
    headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const body = JSON.stringify(data);

    return this.api.delete(`${this.url}/DeleteUser`, { headers, body })
  }


}
