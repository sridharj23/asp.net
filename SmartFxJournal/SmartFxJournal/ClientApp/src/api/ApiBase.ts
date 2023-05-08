import axios, { type AxiosResponse } from 'axios';

export abstract class JournalApi<T> {
    readonly connection;
    protected static readonly apiBase:string = `/MyJournal/api/1.0`;
    protected readonly resourceName:string

    constructor(resource : string) {
        this.connection = axios.create({
            baseURL: JournalApi.apiBase,
            timeout: 300000,
            headers: { 'Content-Type': 'application/json' }
        });
        this.resourceName = (resource.startsWith('/')) ? resource : '/' + resource;
    };

    protected abstract getKeyOf(input : T) : string; 

    protected handleError(e: unknown) {
        console.log(e);
    }

    public async getAll() : Promise<T[]> {
        let result : T[] = new Array<T>;
        try {
            return await this.connection.get(this.resourceName).then(response => response.data as T[]);
        } catch (error) {
            this.handleError(error);
        }
        return result;
    }

    public async get(key: string) : Promise<T> {
        let result : T[] = new Array<T>;
        try {
            return await this.connection.get(this.resourceName + '/' + key).then(response => response.data as T);
        } catch (error) {
            this.handleError(error);
        }
        return null as T;
    }

    public async createNew(input : T) : Promise<T> {
        try {
            console.log(input);
            return await this.connection.post(this.resourceName, input).then(response => response.data as T);
        } catch (error) {
            this.handleError(error);
        }
        return input;
    }

    public async update(input : T) : Promise<T> {
        try {
            return await this.connection.put(this.resourceName + '/' + this.getKeyOf(input) , input).then(response => response.data as T);
        } catch (error) {
            this.handleError(error);
        }
        return input;
    }

    public async delete(input : string | T) : Promise<boolean> {
        try {
            let resKey : string;
            if (typeof input === 'string') {
                resKey = input;
            } else {
                resKey = this.getKeyOf(input);
            }
            return await this.connection.delete(this.resourceName + '/' + resKey).then(response => response.data);
        } catch (error) {
            this.handleError(error);
        }
        return false;
    }
}