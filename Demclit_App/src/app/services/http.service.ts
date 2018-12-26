import { Injectable } from '@angular/core';
import {Http, Request, Response, Headers, RequestOptions} from '@angular/http';
import 'rxjs/Rx';

@Injectable()
export class HttpService {
  response: Response;
  constructor(private http: Http) { }

  getData(url: string, parameters:any = null){
    let paramsData = JSON.stringify(parameters);
    return this.http.get(url).map((response: Response)=>
      response.json()
    )
  }

  sendData(url: string, parameters:any = null){
    let paramsData = JSON.stringify(parameters);
    let headers = new Headers({ 'Content-Type': 'application/json' }); // ... Set content type to JSON
    let options = new RequestOptions({ headers: headers }); // Create a request option
    return this.http.post(url, paramsData, options).map(this.extractData);
  }

  putData(url: string, parameters:any = null){
    let paramsData = JSON.stringify(parameters);
    let headers = new Headers({ 'Content-Type': 'application/json' }); // ... Set content type to JSON
    let options = new RequestOptions({ headers: headers }); // Create a request option
    return this.http.put(url, paramsData, options).map(this.extractData);
  }

  extractData(resp: Response){
    let body = resp.json();
    return body || {};
  }
}
