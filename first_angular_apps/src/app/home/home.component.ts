import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  template: `
    <section>
      <form>
        <input type="text" placeholder="Filter by city" />
          <button class="bg-blue-700 text-white border border-blue-700 rounded-lg px-4 py-2 ">
            Search
          </button>
      </form>
    </section>
  `,
  // templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
