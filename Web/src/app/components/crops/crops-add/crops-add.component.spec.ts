import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CropsAddComponent } from './crops-add.component';

describe('CropsAddComponent', () => {
  let component: CropsAddComponent;
  let fixture: ComponentFixture<CropsAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CropsAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CropsAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
