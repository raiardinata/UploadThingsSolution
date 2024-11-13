import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HousingLocationComponent } from '../housing-location/housing-location.component';
import { HousingLocation } from '../housing-location/interfaces/IHousingLocation';
import { HousingService } from '../housing-location/services/housing.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HousingLocationComponent, CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  readonly baseUrl = 'https://angular.dev/assets/images/tutorials/common';

  filteredLocationList: HousingLocation[] = [];
  housingLocationList: HousingLocation[] = [];
  housingService: HousingService = inject(HousingService);

  filterResults(searchTerm: string) {
    if (!searchTerm) {
      this.filteredLocationList = this.housingLocationList;
      return;
    }

    this.filteredLocationList = this.housingLocationList.filter(
      (housingLocation) => housingLocation?.city
        .toLowerCase()
        .includes(searchTerm.toLowerCase()),
    );
  }

  constructor() {
    this.housingService.getAllHousingLocations().subscribe(
      {
        next: (data) => {
          this.housingLocationList = data.housingLocationData;
          this.filteredLocationList = data.housingLocationData;
        },
        error: (err) => console.error('Error fetching housingLocationList', err),
      }
    );
  }
}
