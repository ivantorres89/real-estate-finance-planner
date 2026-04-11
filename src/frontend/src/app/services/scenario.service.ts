import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ScenarioListItem,
  ScenarioResponse,
  CreateScenarioRequest,
  UpdateScenarioRequest,
  AnalysisResultResponse,
} from '../models';

@Injectable({ providedIn: 'root' })
export class ScenarioService {
  private readonly baseUrl = '/api/scenarios';

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<ScenarioListItem[]> {
    return this.http.get<ScenarioListItem[]>(this.baseUrl);
  }

  getById(id: string): Observable<ScenarioResponse> {
    return this.http.get<ScenarioResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateScenarioRequest): Observable<ScenarioResponse> {
    return this.http.post<ScenarioResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateScenarioRequest): Observable<ScenarioResponse> {
    return this.http.put<ScenarioResponse>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  runAnalysis(id: string): Observable<AnalysisResultResponse> {
    return this.http.post<AnalysisResultResponse>(`${this.baseUrl}/${id}/analyze`, {});
  }
}
