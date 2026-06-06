import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CruiseResult, CabinClass, CancellationPolicy } from '../models/cruise.model';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="results-container">
      <div class="header">
        <h1>Search Results</h1>
        <button class="back-button" (click)="goBack()">← Back to Search</button>
      </div>

      <div class="controls">
        <label for="sortBy">Sort by:</label>
        <select id="sortBy" (change)="onSortChange($event)">
          <option value="price-asc">Price: Low to High</option>
          <option value="price-desc">Price: High to Low</option>
          <option value="provider">Provider</option>
        </select>
      </div>

      <div *ngIf="cruises.length === 0" class="no-results">
        No cruises found matching your search criteria.
      </div>

      <div class="cruises-list">
        <div *ngFor="let cruise of cruises" class="cruise-card">
          <div class="card-header">
            <span class="provider-badge" [ngClass]="cruise.providerId">
              {{ cruise.providerName }}
            </span>
            <h2>{{ cruise.shipName || 'River Cruise' }}</h2>
          </div>

          <div class="card-details">
            <div class="detail-row">
              <span class="label">Cabin Class:</span>
              <span class="value">{{ cruise.cabinClass }}</span>
            </div>

            <div class="detail-row">
              <span class="label">Departure:</span>
              <span class="value">{{ cruise.departurePort }}</span>
            </div>

            <div class="detail-row">
              <span class="label">Sailing Date:</span>
              <span class="value">{{ cruise.sailingDate | date:'MMM dd, yyyy' }}</span>
            </div>

            <div class="detail-row">
              <span class="label">Passengers:</span>
              <span class="value">{{ cruise.passengerCount }}</span>
            </div>

            <div class="detail-row" *ngIf="cruise.inclusions.length > 0">
              <span class="label">Inclusions:</span>
              <span class="value">{{ cruise.inclusions.join(', ') }}</span>
            </div>

            <div class="detail-row">
              <span class="label">Cancellation:</span>
              <span class="value">{{ cruise.cancellationPolicy }}</span>
            </div>
          </div>

          <div class="pricing">
            <div class="price-row">
              <span>Price per Person:</span>
              <span class="price">{{ cruise.pricePerPerson | currency }}</span>
            </div>
            <div class="price-row" *ngIf="cruise.portFees">
              <span>Port Fees:</span>
              <span class="price">{{ cruise.portFees | currency }}</span>
            </div>
            <div class="price-row total">
              <span>Total Price:</span>
              <span class="price">{{ cruise.totalPrice | currency }}</span>
            </div>
          </div>

          <button class="book-button" (click)="bookCruise(cruise)">
            Book Now
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .results-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 20px;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    h1 {
      margin: 0;
      color: #333;
    }

    .back-button {
      padding: 8px 16px;
      background-color: #f0f0f0;
      border: 1px solid #ccc;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
    }

    .back-button:hover {
      background-color: #e0e0e0;
    }

    .controls {
      margin-bottom: 20px;
      display: flex;
      gap: 10px;
      align-items: center;
    }

    .controls select {
      padding: 8px;
      border: 1px solid #ccc;
      border-radius: 4px;
    }

    .no-results {
      text-align: center;
      padding: 40px;
      color: #999;
    }

    .cruises-list {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 20px;
    }

    .cruise-card {
      border: 1px solid #ddd;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      display: flex;
      flex-direction: column;
    }

    .card-header {
      padding: 15px;
      background-color: #f9f9f9;
      border-bottom: 1px solid #eee;
    }

    .provider-badge {
      display: inline-block;
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: 600;
      margin-bottom: 8px;
    }

    .provider-badge.oceanLux {
      background-color: #e3f2fd;
      color: #1976d2;
    }

    .provider-badge.islandHop {
      background-color: #f3e5f5;
      color: #7b1fa2;
    }

    .provider-badge.riverVoyage {
      background-color: #e0f2f1;
      color: #00796b;
    }

    .card-header h2 {
      margin: 0;
      font-size: 18px;
      color: #333;
    }

    .card-details {
      padding: 15px;
      flex-grow: 1;
    }

    .detail-row {
      display: flex;
      justify-content: space-between;
      padding: 5px 0;
      font-size: 14px;
      border-bottom: 1px solid #f0f0f0;
    }

    .detail-row:last-child {
      border-bottom: none;
    }

    .label {
      font-weight: 600;
      color: #666;
    }

    .value {
      color: #333;
    }

    .pricing {
      padding: 15px;
      background-color: #f9f9f9;
      border-top: 1px solid #eee;
      border-bottom: 1px solid #eee;
    }

    .price-row {
      display: flex;
      justify-content: space-between;
      padding: 8px 0;
      font-size: 14px;
    }

    .price-row.total {
      font-weight: 600;
      font-size: 16px;
      padding-top: 10px;
      border-top: 1px solid #ddd;
    }

    .price {
      color: #0066cc;
      font-weight: 600;
    }

    .book-button {
      margin: 15px;
      padding: 12px;
      background-color: #0066cc;
      color: white;
      border: none;
      border-radius: 4px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: background-color 0.3s;
    }

    .book-button:hover {
      background-color: #0052a3;
    }
  `]
})
export class ResultsComponent implements OnInit {
  cruises: CruiseResult[] = [];
  searchParams: any = {};
  private originalCruises: CruiseResult[] = [];

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state) {
      this.searchParams = navigation.extras.state.searchParams;
      this.originalCruises = navigation.extras.state.results || [];
      this.cruises = [...this.originalCruises];
    }
  }

  ngOnInit(): void {
    if (this.cruises.length === 0) {
      this.goBack();
    }
  }

  onSortChange(event: Event): void {
    const sortBy = (event.target as HTMLSelectElement).value;
    this.cruises = [...this.originalCruises];

    switch (sortBy) {
      case 'price-desc':
        this.cruises.sort((a, b) => b.totalPrice - a.totalPrice);
        break;
      case 'provider':
        this.cruises.sort((a, b) => a.providerName.localeCompare(b.providerName));
        break;
      case 'price-asc':
      default:
        this.cruises.sort((a, b) => a.totalPrice - b.totalPrice);
    }
  }

  bookCruise(cruise: CruiseResult): void {
    this.router.navigate(['/booking'], {
      state: {
        cruise: cruise,
        searchParams: this.searchParams
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/search']);
  }
}
