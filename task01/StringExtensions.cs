using System;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string text){

        if (string.IsNullOrEmpty(text)){
            return true;
        }

        int left = 0;
        int right = text.Length - 1;

        while (left < right){

            if (char.IsPunctuation(text[left]) || char.IsWhiteSpace(text[left])){
                left++;
                continue;
            }

            if (char.IsPunctuation(text[right]) || char.IsWhiteSpace(text[right])){
                right--;
                continue;
            }

            if (char.ToLower(text[left]) != char.ToLower(text[right])){
                return false;
            }

            left++;
            right--;
        }
        return true;
    }
}
