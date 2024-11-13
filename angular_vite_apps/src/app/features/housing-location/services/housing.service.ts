import { Injectable } from '@angular/core';
import { GetAllHousingLocation, HousingLocation } from '../interfaces/IHousingLocation';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HousingService {

  readonly url: string = 'https://localhost:7102/v1/HousingLocation';

  constructor(private http: HttpClient) { }

  getAllHousingLocations(): Observable<GetAllHousingLocation> {
    return this.http.get<GetAllHousingLocation>(`${this.url}/GetAllList`).pipe(
      map((response) => response),
      catchError(this.handleError)
    );
  }

  getHousingLocationById(id: number): Observable<HousingLocation | undefined> {
    return this.http.get<HousingLocation | undefined>(`${this.url}/?id=${id}&data_that_needed=id,name,city,state,photo,availableUnits,wifi,laundry`).pipe(
      map((response) => response),
      catchError(this.handleError),
    );
  }

  submitApplication(firstName: string, lastName: string, email: string) {
    console.log(
      `Homes application receive: First Name: ${firstName}, Last Name: ${lastName}, Email: ${email}`
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';

    if (typeof ErrorEvent !== 'undefined' && error.error instanceof ErrorEvent) {
      // Client-side or network error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else if (error.status === 0) {
      errorMessage = `Network error: Verify the backend is running and reachable.`;
    } else {
      errorMessage = `Backend returned code ${error.status}, body was: ${error.message}`;
    }

    console.error('Detailed error:', error);
    return throwError(() => new Error(errorMessage));
  }
}
