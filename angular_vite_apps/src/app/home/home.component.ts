import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HousingLocationComponent } from '../housing-location/housing-location.component';
import { HousingLocation } from '../housinglocation';
import { HousingService } from '../housing.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HousingLocationComponent, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  readonly baseUrl = 'https://angular.dev/assets/images/tutorials/common';

  filteredLocationList: HousingLocation[] = [];
  housingLocationList: HousingLocation[];
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

  onSubmit(event: Event) {
    event.preventDefault(); // Prevents the form from submitting and reloading the page
  }

  constructor() {
    this.housingLocationList = this.housingService.getAllHousingLocation();
    this.filteredLocationList = this.housingLocationList;
  }
}
