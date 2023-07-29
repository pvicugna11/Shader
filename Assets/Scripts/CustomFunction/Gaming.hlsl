void Gaming_float(float time, float term, uint num, out float value)
{
    value = 1 / 2;

    for (uint i = 0; i < num; i++)
    {
        value += 12 * cos(((2 * i + 1) * PI * time) / (3 * term)) / pow((2 * i + 1) * PI, 2);
    }
}
