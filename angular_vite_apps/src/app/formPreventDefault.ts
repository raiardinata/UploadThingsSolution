export class FormPreventDefault {
  onSubmit(event: Event) {
    debugger;
    event.preventDefault(); // Prevents the form from submitting and reloading the page
  }
  constructor() { }
}
