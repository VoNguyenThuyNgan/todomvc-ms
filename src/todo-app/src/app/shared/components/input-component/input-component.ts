import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-input-component',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './input-component.html',
  styleUrl: './input-component.scss',
})
export class InputComponent {
  value = model('');

  placeholder = input('');
  cssClass = input('');
  autofocus = input(false);
  ariaLabel = input('');

  enter = output<void>();
  escape = output<void>();
  blurEvent = output<void>();
}
