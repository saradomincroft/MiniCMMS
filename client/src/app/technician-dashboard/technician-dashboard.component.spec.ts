import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TechnicianDashboardComponent } from './technician-dashboard.component';

describe('TechnicianDashboard', () => {
  let component: TechnicianDashboardComponent;
  let fixture: ComponentFixture<TechnicianDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TechnicianDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TechnicianDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
