import { Injectable } from '@angular/core';
import { HousingLocation } from './housinglocation';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HousingService {

  readonly url: string = 'https://localhost:7102/v1/HousingLocation';

  constructor(private http: HttpClient) { }

  getAllHousingLocations(): Observable<any> {
    return this.http.get<any>(`${this.url}/GetAllList`).pipe(
      map((response) => response),
      catchError(this.handleError)
    );
  }

  getHousingLocationById(id: number): Observable<HousingLocation | undefined> {
    return this.http.get<HousingLocation | undefined>(`${this.url}/?id=${id}&data_that_needed=id,name,city,state,photo,availableUnits,wifi,laundry`).pipe(
      map((response) => response),
      catchError(this.handleError)
    );
  }

  submitApplication(firstName: string, lastName: string, email: string) {
    console.log(
      `Homes application receive: First Name: ${firstName}, Last Name: ${lastName}, Email: ${email}`
    );
  }

  public handleError(error: HttpErrorResponse) {
    // Check if the error is a client-side or network error
    if (error.error instanceof ErrorEvent) {
      console.error('Client-side error:', error.error.message);
      return throwError(() => new Error(`Client-side error: ${error.error.message}`));
    } else {
      // Server-side error
      console.error(`Server error: ${error.status} - ${error.message}`);
      return throwError(() =>
        new Error(`Server error: ${error.status} - ${error.message}`)
      );
    }
  }
}
