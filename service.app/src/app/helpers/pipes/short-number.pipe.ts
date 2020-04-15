import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'shortNumber'
})
export class ShortNumberPipe implements PipeTransform {

  transform(num: number, args?: any): any {
    if (num <= 999) {
      return num;
    }
    // thousands
    else if (num >= 1000 && num <= 999999) {
      return (num / 1000) + 'K';
    }
    // millions
    else if (num >= 1000000 && num <= 999999999) {
      return (num / 1000000) + 'M';
    }
    // billions
    else if (num >= 1000000000 && num <= 999999999999) {
      return (num / 1000000000) + 'B';
    }
    else
      return num;
  }
}


